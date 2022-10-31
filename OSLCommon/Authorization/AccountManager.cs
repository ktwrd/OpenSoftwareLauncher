using kate.shared.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OSLCommon.Authorization
{
    public delegate void AccountDelegate(Account account);
    public partial class AccountManager
    {
        public List<Account> AccountList = new List<Account>();

        public static List<ITokenGranter> TokenGranters = new List<ITokenGranter>()
        {
        };
        public static string[] DefaultLicenses = Array.Empty<string>();
        public static void RegisterTokenGranter(ITokenGranter granter)
        {
            foreach (var item in TokenGranters)
                if (item == granter)
                    return;
            TokenGranters.Add(granter);
        }

        #region Events
        public bool IsPendingWrite { get; private set; }
        public event VoidDelegate PendingWrite;
        public event AccountDelegate AccountUpdated;
        internal void OnAccountUpdate(Account account)
        {
            if (AccountUpdated != null)
                AccountUpdated?.Invoke(account);
        }
        public void ForcePendingWrite()
        {
            IsPendingWrite = true;
            if (PendingWrite != null)
                PendingWrite?.Invoke();
        }
        internal void OnPendingWrite()
        {
            IsPendingWrite = true;
            if (PendingWrite != null)
                PendingWrite?.Invoke();
        }
        public void ClearPendingWrite()
        {
            foreach (var item in AccountList)
                item.ClearPendingWrite();
            IsPendingWrite = false;
        }
        #endregion

        public virtual bool ValidateToken(string token)
        {
            foreach (var account in AccountList)
            {
                foreach (var item in account.Tokens)
                    if (item.Token == token && account.Enabled)
                        return true;
            }
            return false;
        }

        public virtual void SetUserGroups(Dictionary<string, string[]> dict)
        {
            foreach (var acc in this.AccountList)
            {
                if (dict.ContainsKey(acc.Username))
                {
                    acc.Groups = new List<string>(dict[acc.Username]).ToArray();
                }
            }
            OnPendingWrite();
        }

        /// <summary>
        /// Check if account is invulnerable.
        /// </summary>
        /// <param name="account">Is <see cref="Nullable{Account}"/></param>
        public virtual bool IsInvulnerable(Account account)
        {
            return account == null ? false : account.HasPermission(AccountPermission.INVULNERABLE);
        }

        #region Get Account
        public virtual Account GetAccount(string token, bool bumpLastUsed=false)
        { 
            foreach (var account in AccountList)
            {
                if (account.HasToken(token))
                {
                    if (bumpLastUsed)
                        TokenUsed(token);
                    return account;
                }
            }
            return null;
        }
        public virtual Account GetAccountByUsername(string username)
        {
            foreach (var account in AccountList)
            {
                if (account.Username == username)
                    return account;
            }
            return null;
        }
        public enum AccountField
        {
            Username
        }
        public virtual Account[] GetAccountsByRegex(Regex expression, AccountField field=AccountField.Username)
        {
            var list = new LinkedList<Account>();
            foreach (var account in AccountList)
            {
                switch (field)
                {
                    case AccountField.Username:
                        if (!expression.Match(account.Username).Success)
                            continue;
                        break;
                    default:
                        continue;
                        break;
                }

                list.AddLast(account);
            }
            return list.ToArray();
        }
        #endregion

        #region Token Management
        /// <summary>
        /// Used to mark <see cref="AccountToken.LastUsed"/> to the current Unix Epoch
        /// </summary>
        /// <param name="token">Token to set <see cref="AccountToken.LastUsed"/></param>
        public virtual void TokenUsed(string token)
        {
            var account = GetAccount(token);
            if (account == null) return;

            for (int i = 0; i < account.Tokens.Length; i++)
            {
                if (account.Tokens[i] != null)
                    account.Tokens[i] = TokenMarkLastUsedTimestamp(account.Tokens[i]);
            }
            if (!IsPendingWrite)
                OnPendingWrite();
        }
        internal virtual AccountToken TokenMarkLastUsedTimestamp(AccountToken token)
        {
            if (token.Allow)
            {
                token.LastUsed = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }
            return token;
        }

        internal virtual Account CreateAccount()
        {
            return new Account(this);
        }

        /// <summary>
        /// Grant new token from account
        /// </summary>
        /// <param name="account">Target account</param>
        /// <returns></returns>
        public GrantTokenResponse CreateToken(Account account, string userAgent = "", string host = "")
        {
            bool accountFound = false;
            foreach (var item in AccountList)
            {
                if (item == account && account != null)
                {
                    accountFound = true;
                    if (item.Enabled)
                    {
                        var token = new AccountToken(item);
                        AccountToken success = item.AddToken(token);
                        if (success == null)
                            return new GrantTokenResponse(ServerStringResponse.AccountTokenGrantFailed, false);
                        else
                        {
                            if (!IsPendingWrite)
                                OnPendingWrite();
                            if (account.Groups == null)
                                account.Groups = Array.Empty<string>();
                            if (account.Permissions == null)
                                account.Permissions = Array.Empty<AccountPermission>();
                            TokenUsed(success.Token);
                            foreach (var thing in account.Tokens)
                            {
                                if (thing.Token == success.Token)
                                {
                                    thing.UserAgent = userAgent;
                                    thing.Host = host;
                                }
                            }
                            return new GrantTokenResponse(ServerStringResponse.AccountTokenGranted, true, success, account.Groups.ToArray(), account.Permissions.ToArray());
                        }
                    }
                    return new GrantTokenResponse(ServerStringResponse.AccountDisabled, false);
                }
            }

            if (!accountFound)
                return new GrantTokenResponse(ServerStringResponse.AccountNotFound, false);
            return new GrantTokenResponse(ServerStringResponse.AccountTokenGrantFailed, false);
        }

        private GrantTokenResponse AttemptGrantToken(Account account, ITokenGranter granter, string username, string password, string userAgent = "", string host = "")
        {
            var success = granter.Grant(username, password);
            if (success)
            {
                return CreateToken(account, userAgent: userAgent, host: host);
            }
            return null;
        }
        public GrantTokenResponse GrantToken(Account account, string username, string password, string userAgent="", string host="")
        {
            var taskList = new List<Task>();
            GrantTokenResponse targetResponse = null;
            foreach (var granter in TokenGranters)
            {
                taskList.Add(new Task(delegate
                {
                    var res = AttemptGrantToken(account, granter, username, password, userAgent: userAgent, host: host);
                    if (targetResponse == null && res != null)
                        targetResponse = res;
                }));
            }
            foreach (var i in taskList)
                i.Start();
            Task.WhenAll(taskList).Wait();
            if (targetResponse == null)
                return new GrantTokenResponse("Failed to grant token", false);
            return targetResponse;
        }

        public GrantTokenResponse GrantTokenAndOrAccount(string username, string password, string userAgent="", string host="")
        {
            foreach (var account in AccountList)
            {
                // Found our account
                if (account.Username == username)
                {
                    return GrantToken(account, username, password, userAgent: userAgent, host: host);
                }
            }

            // Create account
            var accountInstance = CreateAccount();
            accountInstance.Username = username;
            AccountList.Add(accountInstance);
            var response = GrantToken(accountInstance, username, password, userAgent: userAgent, host: host);
            if (!response.Success)
                AccountList.Remove(accountInstance);
            return response;
        }
        #endregion

        #region Account Permission
        /// <summary>
        /// 
        /// </summary>
        /// <param name="account">Account to check</param>
        /// <param name="permissions">Array of permissions to check</param>
        /// <param name="ignoreAdmin">When <see cref="Account"/> has the <see cref="AccountPermission.ADMINISTRATOR"/> permission, then <see cref="true"/> is always returned.</param>
        /// <returns></returns>
        public virtual bool AccountHasPermission(Account account, AccountPermission[] permissions, bool ignoreAdmin=false)
        {
            bool match = false;
            foreach (var target in account.Permissions)
            {
                if (permissions.Contains(target))
                    match = true;
            }
            if (account.Permissions.Contains(AccountPermission.ADMINISTRATOR) && !ignoreAdmin)
                return true;
            return match;
        }
        /// <summary>
        /// Singular Permission overload for <see cref="AccountHasPermission(Account, AccountPermission[], bool)"/>
        /// </summary>
        public bool AccountHasPermission(
            Account account,
            AccountPermission permission,
            bool ignoreAdmin = false)
                => AccountHasPermission(
                    account,
                    new AccountPermission[] { permission },
                    ignoreAdmin);
        /// <summary>
        /// Token overload for <see cref="AccountHasPermission(Account, AccountPermission[], bool)"/>
        /// </summary>
        public virtual bool AccountHasPermission(string token, AccountPermission[] permissions, bool ignoreAdmin=false, bool bumpLastUsed=false)
        {
            if (token == null || token.Length < AccountToken.TokenLength || token.Length > AccountToken.TokenLength) return false;
            var account = GetAccount(token);
            if (account == null || !account.Enabled) return false;
            if (bumpLastUsed)
                TokenUsed(token);
            return AccountHasPermission(account, permissions, ignoreAdmin);
        }
        /// <summary>
        /// Singular Permission overload for <see cref="AccountHasPermission(string, AccountPermission[], bool)"/>
        /// </summary>
        public bool AccountHasPermission(
            string token,
            AccountPermission permission,
            bool ignoreAdmin = false)
                => AccountHasPermission(
                    token,
                    new AccountPermission[] { permission },
                    ignoreAdmin);
        #endregion

        #region JSON (Des|S)erialization
        public void ReadJSON(string jsonContent)
        {
            if (jsonContent.Length < 1)
                Trace.WriteLine($"[AccountManager->Read:{GeneralHelper.GetNanoseconds()}] [ERR] Argument jsonContent has invalid length of {jsonContent.Length}");
            if (!RegExStatements.JSON.Match(jsonContent).Success)
                Trace.WriteLine($"[AccountManager->Read:{GeneralHelper.GetNanoseconds()}] [ERR] Argument jsonContent failed RegExp validation\n--------\n{jsonContent}\n--------\n");
            var jsonOptions = new JsonSerializerOptions()
            {
                IgnoreReadOnlyFields = false,
                IgnoreReadOnlyProperties = false,
                IncludeFields = true
            };
            var deserialized = JsonSerializer.Deserialize<List<Account>>(jsonContent, jsonOptions);
            if (deserialized == null)
            {
                Trace.WriteLine($"[AccountManager->Read:{GeneralHelper.GetNanoseconds()}] [WARN] Deserialized List<Account> is null. Content is\n--------\n{jsonContent}\n--------\n");
            }
            foreach (var item in deserialized)
            {
                if (item.accountManager == null)
                    item.accountManager = this;
            }
            AccountList = deserialized;
            Trace.WriteLine($"[AccountManager->Read:{GeneralHelper.GetNanoseconds()}] Deserialized Accounts ({AccountList.Count} Accounts)");
        }

        public string ToJSON()
        {
            var jsonOptions = new JsonSerializerOptions()
            {
                IgnoreReadOnlyFields = false,
                IgnoreReadOnlyProperties = false,
                IncludeFields = true,
                WriteIndented = true
            };
            try
            {
                var serialized = JsonSerializer.Serialize(AccountList, jsonOptions);
                return serialized;
            }
            catch (Exception except)
            {
                string txt = $"[AccountManager->Read:{GeneralHelper.GetNanoseconds()}] [ERR] Failed to serialize field AccountList\n--------\n{except}\n--------\n";
                Trace.WriteLine(txt);
                Console.Error.WriteLine(txt);
                throw;
            }
        }
        #endregion
    }
}
