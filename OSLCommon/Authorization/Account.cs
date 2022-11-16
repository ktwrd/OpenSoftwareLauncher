using kate.shared.Helpers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace OSLCommon.Authorization
{
    public class Account
    {
        #region Constructors
        public Account(AccountManager accountManager)
        {
            this.accountManager = accountManager;
        }
        public Account() : this(null)
        { }
        #endregion

        [BsonIgnore]
        [JsonIgnore]
        [SoapIgnore]
        [XmlIgnore]
        internal bool eventHook = false;
        /// <summary>
        /// Merge account data into this one.
        /// </summary>
        public void Merge(Account sourceAccount)
        {
            tokens = sourceAccount.tokens;
            permissions = sourceAccount.permissions;
            groups = sourceAccount.groups;
            enabled = sourceAccount.enabled;
            disableReasons = sourceAccount.disableReasons;
            licenses = sourceAccount.licenses;
        }

        #region Fields
        internal AccountManager accountManager = null;
        [Browsable(false)]
        public ObjectId _id { get; set; }
        public string Username { get; set; } = "";
        private AccountToken[] tokens = Array.Empty<AccountToken>();
        public AccountToken[] Tokens
        {
            get => tokens;
            set
            {
                tokens = value;
                if (accountManager != null)
                    accountManager.OnAccountUpdate(this);
            }
        }
        internal AccountPermission[] permissions = Array.Empty<AccountPermission>();
        public AccountPermission[] Permissions
        {
            get => permissions;
            set
            {
                permissions = value;
                if (accountManager != null)
                    accountManager.OnAccountUpdate(this);
            }
        }
        internal string[] groups = Array.Empty<string>();
        public string[] Groups
        {
            get => groups;
            set
            {
                groups = value;
                if (accountManager != null)
                    accountManager.OnAccountUpdate(this);
            }
        }

        internal bool enabled = true;
        /// <summary>
        /// Setting this to false will deny this account from accessing any endpoints.
        /// </summary>
        public bool Enabled
        {
            get => enabled;
            set
            {
                enabled = value;
                if (accountManager != null)
                    accountManager.OnAccountUpdate(this);
            }
        }

        internal AccountDisableReason[] disableReasons = Array.Empty<AccountDisableReason>();
        /// <summary>
        /// Reasons why this account was disabled.
        /// </summary>
        public AccountDisableReason[] DisableReasons
        {
            get => disableReasons;
            set
            {
                disableReasons = value;
                if (accountManager != null)
                    accountManager.OnAccountUpdate(this);
            }
        }

        /// <summary>
        /// Timestamp of the first token for this account.
        /// </summary>
        public long FirstSeenTimestamp
        {
            get
            {
                long timestamp = long.MaxValue;
                foreach (var token in Tokens)
                {
                    if (token.CreatedTimestamp < timestamp)
                        timestamp = token.CreatedTimestamp;
                }
                return timestamp;
            }
            set { }
        }
        /// <summary>
        /// Timestamp of the token that was last created.
        /// </summary>
        public long LastSeenTimestamp
        {
            get
            {
                long timestamp = long.MinValue;
                foreach (var token in Tokens)
                {
                    if (token.CreatedTimestamp > timestamp)
                        timestamp = token.CreatedTimestamp;
                    if (token.LastUsed > timestamp)
                        timestamp = token.LastUsed;
                }
                return timestamp;
            }
            set { }
        }

        internal string[] licenses = Array.Empty<string>();
        public string[] Licenses
        {
            get
            {
                return licenses;
            }
            set
            {
                licenses = value;
                if (this.accountManager != null)
                    this.accountManager.OnAccountUpdate(this);
            }
        }

        /// <summary>
        /// Is there new data that doesn't exist locally.
        /// </summary>
        [JsonIgnore]
        public bool PendingWrite
        {
            get => _pendingWrite;
            set
            {
                _pendingWrite = value;
                if (value && accountManager != null)
                    accountManager.OnPendingWrite();
            }
        }
        private bool _pendingWrite = false;
        
        #endregion

        #region License Management

        /// <returns>true: License exists. false: License does not exist</returns>
        public bool HasLicense(string remoteSignature, bool ignoreAdmin = false, bool ignoreDefault = false)
        {
            if (!ignoreDefault && AccountManager.DefaultLicenses.Contains(remoteSignature))
                return true;
            if (!ignoreAdmin && Permissions.Contains(AccountPermission.ADMINISTRATOR))
                return true;
            return Licenses.Contains(remoteSignature);
        }
        /// <returns>true: License does not exist and was added. false: License already exists, ignoring.</returns>
        public bool GrantLicense(string remoteSignature)
        {
            if (Licenses.Contains(remoteSignature))
                return false;
            Licenses = Licenses.Concat(new string[] { remoteSignature }).ToArray();
            PendingWrite = true;
            return true;
        }
        /// <returns>true: License does not exist and was added. false: License already exists, ignoring.</returns>
        public bool RevokeLicense(string remoteSignature)
        {
            if (!Licenses.Contains(remoteSignature))
                return false;
            var tmpList = new List<string>(Licenses);
            tmpList.Remove(remoteSignature);
            Licenses = tmpList.ToArray();
            PendingWrite = true;
            return true;
        }
        #endregion

        #region Group Management
        /// <summary>
        /// Grant this account a group.
        /// </summary>
        /// <param name="group">Formatted to uppercase and trimmed</param>
        /// <returns>Is this account in the supplied group already</returns>
        public bool AddGroup(string group)
        {
            foreach (var item in Groups.ToArray())
                if (item == group.ToUpper().Trim())
                    return true;
            Groups = Groups.Concat(new string[] { group.ToUpper().Trim() }).ToArray();
            PendingWrite = true;
            return false;
        }
        /// <summary>
        /// Check if this account is in this group. Useful for checking if an account has a license for a product.
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public bool HasGroup(string group)
        {
            foreach (var item in Groups.ToArray())
                if (item == group.ToUpper().Trim())
                    return true;
            return false;
        }

        public bool RevokeGroup(string group)
        {
            int previousLength = Groups.Length;
            var res = Groups.Where(v => v != group.ToUpper().Trim()).ToArray();
            Groups = res;
            return previousLength != Groups.Length;
        }
        #endregion

        #region Token Management
        /// <summary>
        /// Remove a singular token from this account.
        /// </summary>
        /// <param name="token">Token to remove</param>
        public void RemoveToken(string token) => RemoveToken(new string[] { token });

        /// <summary>
        /// Remove an array of matching tokens from this account
        /// </summary>
        /// <param name="tokens">Tokens to remove</param>
        public void RemoveToken(string[] tokens)
        {
            var newTokenList = new List<AccountToken>();
            foreach (var item in Tokens.ToArray())
            {
                if (!tokens.Contains(item.Token))
                    newTokenList.Add(item);
            }
            Tokens = newTokenList.ToArray();
            PendingWrite = true;

        }

        /// <summary>
        /// Remove all tokens from this user
        /// </summary>
        /// <returns>Amount of tokens that were removed.</returns>
        public int RemoveTokens()
        {
            int count = Tokens.Length;
            Tokens = Array.Empty<AccountToken>();
            PendingWrite = true;
            return count;
        }

        /// <summary>
        /// Remove all tokens except some.
        /// </summary>
        /// <param name="exclude">Array of tokens to not delete</param>
        /// <returns>Amount of tokens that were removed</returns>
        public int RemoveTokens(string[] exclude=null)
        {
            int count = Tokens.Length;
            var newTokenList = new List<AccountToken>();
            foreach (var item in Tokens.ToArray())
            {
                if (exclude != null && exclude.Contains(item.Token))
                {
                    newTokenList.Add(item);
                }
            }
            Tokens = newTokenList.ToArray();
            PendingWrite = true;
            return count - newTokenList.Count;
        }

        /// <summary>
        /// Register a token
        /// </summary>
        /// <param name="targetToken"></param>
        /// <returns><see cref="null"/> if the account is disabled or there was no token given or if the token given has no parent. <see cref="AccountToken"/> if the token exists already or it was a success.</returns>
        public AccountToken AddToken(AccountToken targetToken)
        {
            if (targetToken == null) return null;
            if (Enabled)
            {
                foreach (var token in Tokens.ToArray())
                {
                    if (token == targetToken)
                        return token;
                }
                targetToken.CreatedTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var newTokens = Tokens.Concat(new AccountToken[]
                {
                    targetToken
                }).ToArray();
                Tokens = newTokens;
                PendingWrite = true;
                Trace.WriteLine($"[Account->AddToken:{GeneralHelper.GetNanoseconds()}] Granted token for {Username}");
                return targetToken;
            }
            return null;
        }

        /// <summary>
        /// Is this token registered to this account.
        /// </summary>
        /// <param name="token">Token to look for</param>
        /// <returns>Is this token registered to this account</returns>
        public bool HasToken(string token)
        {
            foreach (var item in Tokens.ToArray())
            {
                if (item.Token == token && item.Allow)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Fetch details about the account tokens without sensitive information.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public AccountTokenDetailsResponse GetTokenDetails(string token)
        {
            foreach (var item in Tokens.ToArray())
            {
                if (item.Token == token)
                {
                    return new AccountTokenDetailsResponse()
                    {
                        Username = this.Username,
                        Enabled = this.Enabled,
                        CreatedTimestamp = item.CreatedTimestamp,
                        LastUsed = item.LastUsed,
                        UserAgent = item.UserAgent,
                        Host = item.Host,
                        Hash = item.TokenHash
                    };
                }
            }
            return null;
        }

        public AccountTokenDetailsResponse[] GetTokenDetails()
        {
            var responseList = new List<AccountTokenDetailsResponse>();
            foreach (var item in Tokens.ToArray())
            {
                responseList.Add(new AccountTokenDetailsResponse()
                {
                    Username = this.Username,
                    Enabled = this.Enabled,
                    CreatedTimestamp = item.CreatedTimestamp,
                    LastUsed = item.LastUsed,
                    UserAgent = item.UserAgent,
                    Host = item.Host,
                    Hash = item.TokenHash
                });
            }
            return responseList.ToArray();
        }

        #endregion

        #region Account Disable
        /// <summary>
        /// Disable this account. This can be re-enabled by an administrator
        /// </summary>
        /// <param name="reason">Reason to give the user when they attempt to authorize.</param>
        public void DisableAccount(string reason = "No reason")
        {
            Enabled = false;
            DisableReasons = DisableReasons.Concat(new AccountDisableReason[]
            {
                new AccountDisableReason()
                {
                    Message = reason,
                    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                }
            }).ToArray();
        }

        /// <summary>
        /// Re-enable account and cleaning all disable reasons, effecetively the same as unbanning someone from a tf2 server.
        /// </summary>
        public void Pardon()
        {
            Enabled = true;
            CleanDisableReasons();
        }

        /// <summary>
        /// Remove all reasons why their account was disabled
        /// </summary>
        public void CleanDisableReasons()
        {
            Trace.WriteLine($"[Account->CleanDisableReasons:{GeneralHelper.GetNanoseconds()}] {Username}");
            DisableReasons = Array.Empty<AccountDisableReason>();
            PendingWrite = true;
        }
        #endregion

        #region Permission Management
        /// <summary>
        /// Grant to the account a permission if they don't have it already.
        /// </summary>
        /// <param name="target">Permission to add to the user</param>
        /// <returns>If the user has the permission already</returns>
        public bool GrantPermission(AccountPermission target)
        {
            foreach (var perm in Permissions.ToArray())
                if (perm == target)
                    return true;
            var tmpList = new List<AccountPermission>(Permissions);
            tmpList.Add(target);
            Permissions = tmpList.ToArray();
            PendingWrite = true;
            return false;
        }

        /// <summary>
        /// Check if the account has a certian permission
        /// </summary>
        /// <param name="target">Permission to look for.</param>
        /// <returns><see cref="true"/> if the account has the permission, <see cref="false"/> if they do not have it.</returns>
        public bool HasPermission(AccountPermission target)
        {
            foreach (var item in Permissions.ToArray())
                if (item == target)
                    return true;
            return false;
        }

        /// <summary>
        /// Remove a permission from this account
        /// </summary>
        /// <param name="target"></param>
        /// <returns>Did this account already have the target permission?</returns>
        public bool RevokePermission(AccountPermission target)
        {
            bool found = false;
            var newPermissionArray = new List<AccountPermission>();
            foreach (var item in Permissions.ToArray())
            {
                if (item != target)
                {
                    newPermissionArray.Add(item);
                }
                else
                {
                    found = true;
                }
            }
            Permissions = newPermissionArray.ToArray();
            PendingWrite = true;
            return found;
        }
        #endregion


        internal void ClearPendingWrite()
        {
            _pendingWrite = false;
        }

        /// <summary>
        /// Get a summary of the user, without any tokens. This is safe to give to an end user or an administrator.
        /// </summary>
        /// <returns>Generated Response</returns>
        public AccountDetailsResponse GetDetails()
        {
            if (Groups == null)
            {
                Groups = Array.Empty<string>();
            }
            return new AccountDetailsResponse()
            {
                Username = this.Username ?? "",
                Enabled = this.Enabled,
                Permissions = this.Permissions.ToArray(),
                DisableReasons = this.DisableReasons.ToArray(),
                Licenses = this.Licenses,
                Groups = this.Groups.ToArray(),
                FirstSeenTimestamp = this.FirstSeenTimestamp,
                LastSeenTimestamp = this.LastSeenTimestamp,
                Tokens = GetTokenDetails()
            };
        }
    }
}
