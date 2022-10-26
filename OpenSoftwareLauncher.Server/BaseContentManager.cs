using OSLCommon;
using OSLCommon.Authorization;
using OSLCommon.AutoUpdater;
using OSLCommon.Licensing;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenSoftwareLauncher.Server
{
    public abstract class BaseContentManager
    {
        internal abstract Task Deserialize();
        internal abstract Task Serialize();

        /// <summary>
        /// <see cref="AccountManager.AccountList"/>
        /// </summary>
        /// <returns></returns>
        public abstract Task<Account[]> GetAccount();
        /// <summary>
        /// <see cref="AccountManager.GetAccountByUsername(string)"/>
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public abstract Task<Account> GetAccountByUsername(string username);
        /// <summary>
        /// <see cref="AccountManager.GetAccount(string, bool)"/>
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public abstract Task<Account> GetAccountByToken(string token, bool bumpLastUsed=false);

        #region Token
        /// <summary>
        /// <see cref="AccountManager.AccountHasPermission(string, AccountPermission[], bool, bool)"/>
        /// </summary>
        public abstract Task<bool> AccountHasPermission(
            string username,
            AccountPermission[] permissions,
            bool ignoreAdmin = false);
        /// <summary>
        /// <see cref="AccountManager.TokenMarkLastUsedTimestamp(AccountToken)"/>
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        internal abstract Task<AccountToken> TokenMarkLastUsedTimestamp(
            string token);
        /// <summary>
        /// <see cref="AccountManager.ValidateToken(string)"/>
        /// </summary>
        public abstract Task<bool> TokenExists(string token);
        /// <summary>
        /// <see cref="AccountManager.TokenUsed(string)"/>
        /// </summary>
        public abstract Task TokenUsed(string token);
        /// <summary>
        /// <see cref="AccountManager.GrantTokenAndOrAccount(string, string, string, string)"/>
        /// </summary>
        public abstract Task<GrantTokenResponse> AccountGrantToken(
            string username,
            string password,
            string userAgent = "",
            string host = "",
            bool createAccountIfNotInDatabase = true);
        #endregion
        #region License
        /// <summary>
        /// <see cref=""/>
        /// </summary>
        public abstract Task AccountLicenseGrant(string username, string product);
        public abstract Task AccountLicenseGrant(Dictionary<string, string[]> dict);
        public abstract Task AccountLicenseRevoke(string username, string product);
        public abstract Task AccountLicenseRevoke(Dictionary<string, string[]> dict);
        #endregion
    }
}
