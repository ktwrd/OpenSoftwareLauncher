using kate.shared.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace OSLCommon.Authorization
{
    public class AccountDisableReason
    {
        public string Message = "";
        public long Timestamp = 0;
    }
    public class AccountTokenDetailsResponse
    {
        public string Username { get; set; }
        public bool Enabled { get; set; }
        public long CreatedTimestamp { get; set; }
        public long LastUsed { get; set; }
        public string UserAgent { get; set; }
        public string Host { get; set; }
        public string Hash { get; set; }
    }
    public class AccountDetailsResponse
    {
        public string Username { get; set; }
        public bool Enabled { get; set; }
        public AccountPermission[] Permissions { get; set; }
        public AccountDisableReason[] DisableReasons { get; set; }
        public string[] Licenses { get; set; }
        public string[] Groups { get; set; }
        public long FirstSeenTimestamp { get; set; }
        public long LastSeenTimestamp { get; set; }
        public AccountTokenDetailsResponse[] Tokens { get; set; }
    }
    public class Account
    {
        #region Constructors
        public Account(AccountManager accountManager)
        {
            this.accountManager = accountManager;

            Username = "";
            Tokens = new List<AccountToken>();
            Permissions = new List<AccountPermission>();
            Groups = new List<string>();
            DisableReasons = new List<AccountDisableReason>();
            Enabled = true;
            PendingWrite = false;
        }
        public Account() : this(null)
        { }
        #endregion

        #region Fields
        internal AccountManager accountManager = null;
        public string Username { get; set; }
        public List<AccountToken> Tokens { get; set; }
        public List<AccountPermission> Permissions { get; set; }
        public List<string> Groups { get; set; }

        /// <summary>
        /// Setting this to false will deny this account from accessing any endpoints.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Reasons why this account was disabled.
        /// </summary>
        public List<AccountDisableReason> DisableReasons { get; set; }

        /// <summary>
        /// Timestamp of the first token for this account.
        /// </summary>
        public long FirstSeenTimestamp
        {
            get
            {
                long timestamp = long.MaxValue;
                foreach (var token in Tokens.ToArray())
                {
                    if (token.CreatedTimestamp < timestamp)
                        timestamp = token.CreatedTimestamp;
                }
                return timestamp;
            }
        }
        /// <summary>
        /// Timestamp of the token that was last created.
        /// </summary>
        public long LastSeenTimestamp
        {
            get
            {
                long timestamp = long.MinValue;
                foreach (var token in Tokens.ToArray())
                {
                    if (token.CreatedTimestamp > timestamp)
                        timestamp = token.CreatedTimestamp;
                    if (token.LastUsed > timestamp)
                        timestamp = token.LastUsed;
                }
                return timestamp;
            }
        }


        /// <summary>
        /// Is there new data that doesn't exist locally.
        /// </summary>
        public bool PendingWrite
        {
            get => _pendingWrite;
            private set
            {
                _pendingWrite = value;
                if (value)
                    accountManager.OnPendingWrite();
            }
        }
        private bool _pendingWrite = false;

        #endregion

        #region License Management
        public List<string> Licenses { get; set; } = new List<string>();

        /// <returns>true: License exists. false: License does not exist</returns>
        public bool HasLicense(string remoteSignature, bool ignoreAdmin = false)
        {
            if (AccountManager.DefaultLicenses.Contains(remoteSignature))
                return true;
            if (!ignoreAdmin && Permissions.Contains(AccountPermission.ADMINISTRATOR))
                return true;
            return Licenses.Contains(remoteSignature);
        }
        /// <returns>true: License does not exist and was added. false: Licence already exists, ignoring.</returns>
        public bool GrantLicense(string remoteSignature)
        {
            if (Licenses.ToArray().Contains(remoteSignature))
                return false;
            Licenses.Add(remoteSignature);
            PendingWrite = true;
            return true;
        }
        /// <returns>true: License does not exist and was added. false: Licence already exists, ignoring.</returns>
        public bool RevokeLicense(string remoteSignature)
        {
            if (!Licenses.ToArray().Contains(remoteSignature))
                return false;
            Licenses.Remove(remoteSignature);
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
            Groups.Add(group.ToUpper().Trim());
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
            var res = Groups.Remove(group.ToUpper().Trim());
            PendingWrite = res;
            return res;
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
            Tokens = newTokenList;
            PendingWrite = true;

        }

        /// <summary>
        /// Remove all tokens from this user
        /// </summary>
        /// <returns>Amount of tokens that were removed.</returns>
        public int RemoveTokens()
        {
            int count = Tokens.Count;
            Tokens = new List<AccountToken>();
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
            int count = Tokens.Count;
            var newTokenList = new List<AccountToken>();
            foreach (var item in Tokens.ToArray())
            {
                if (exclude != null && exclude.Contains(item.Token))
                {
                    newTokenList.Add(item);
                }
            }
            Tokens = newTokenList;
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
            if (targetToken.parentAccount != this) return null;
            if (Enabled)
            {
                foreach (var token in Tokens.ToArray())
                {
                    if (token == targetToken)
                        return token;
                }
                targetToken.CreatedTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                Tokens.Add(targetToken);
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
                if (item.Token == token)
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
            DisableReasons.Add(new AccountDisableReason()
            {
                Message = reason,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            });
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
            DisableReasons.Clear();
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
            Permissions.Add(target);
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
            Permissions = newPermissionArray;
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
                Groups = new List<string>();
                PendingWrite = true;
            }
            return new AccountDetailsResponse()
            {
                Username = this.Username ?? "",
                Enabled = this.Enabled,
                Permissions = this.Permissions.ToArray(),
                DisableReasons = this.DisableReasons.ToArray(),
                Licenses = this.Licenses.ToArray(),
                Groups = this.Groups.ToArray(),
                FirstSeenTimestamp = this.FirstSeenTimestamp,
                LastSeenTimestamp = this.LastSeenTimestamp,
                Tokens = GetTokenDetails()
            };
        }
    }
}
