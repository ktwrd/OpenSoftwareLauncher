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

        public abstract Task<Account[]> GetAccount();
        public abstract Task<Account> GetAccountByUsername(string username);
        public abstract Task<Account> GetAccountByToken(string token);

        #region Token
        public abstract Task<bool> AccountHasPermission(
            string username,
            AccountPermission[] permissions,
            bool ignoreAdmin = false);
        internal abstract Task<AccountToken> TokenMarkLastUsedTimestamp(
            string token);
        public abstract Task<bool> TokenExists(string token);
        #endregion
        public abstract Task<GrantTokenResponse> AccountGrantToken(
            string username,
            string password,
            string userAgent = "",
            string host = "",
            bool createAccountIfNotInDatabase = true);
        #region License
        public abstract Task AccountLicenseGrant(string username, string product);
        public abstract Task AccountLicenseGrant(Dictionary<string, string[]> dict);
        public abstract Task AccountLicenseRevoke(string username, string product);
        public abstract Task AccountLicenseRevoke(Dictionary<string, string[]> dict);
        #endregion

        public abstract Task SetAccount(Account account);
    }
}
