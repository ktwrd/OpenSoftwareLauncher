using kate.shared.Helpers;
using OSLCommon.Authorization;

using System.Net;
using System.Text.Json;

namespace OSLCommon
{
    public class Endpoint
    {
        public string BaseURL { get; set; }
        public event StringDelegate BaseAPIChanged;
        public void OnBaseAPIChanged(string route)
        {
            if (BaseAPIChanged != null)
                BaseAPIChanged.Invoke(route);
        }
        private string encode(object content)
        {
            return WebUtility.UrlEncode(content.ToString());
        }
        public JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
        {
            IncludeFields = true
        };

        public string ServerDetails
            => $"{BaseURL}/server/details";
        public string AccountDetails(string token)
            => $"{BaseURL}/account?token={encode(token)}";
        #region Release
        public string LatestReleaseBase
            => $"{BaseURL}/release/latest";
        public string LatestRelease(string id)
            => $"{LatestReleaseBase}/{id}";
        public string LatestRelease(string id, string token)
            => $"{LatestReleaseBase}/{id}?token={encode(token)}";
        public string Files(string hash)
            => $"{BaseURL}/file?hash={encode(hash)}";
        public string AvailableProducts()
            => $"{BaseURL}/products/available";
        public string AvailableProducts(string token)
            => $"{AvailableProducts()}?token={encode(token)}";
        #endregion

        #region Token
        public string TokenGrant(string username, string password)
            => $"{BaseURL}/token/grant?username={encode(username)}&password={encode(password)}";
        public string TokenValidate(string token)
            => $"{BaseURL}/token/validate?token={encode(token)}";
        public string TokenDetails(string token)
            => $"{BaseURL}/token/details?token={encode(token)}";
        public string TokenRemove(string token, bool all = false)
            => $"{BaseURL}/token/remove?token={encode(token)}&all={encode(all)}";
        #endregion


        #region Admin
        #region Announcement
        public string AnnouncementLatest()
            => $"{BaseURL}/admin/announcement/latest";
        public string AnnouncementCreate(string token, string content, bool active = true)
            => $"{BaseURL}/admin/announcement/new?token={encode(token)}&content={encode(content)}&active={encode(active.ToString())}";
        public string AnnouncementUpdateCurrent(string token, string content, bool active = true)
            => $"{BaseURL}/admin/announcement/update?token={encode(token)}&content={encode(content)}&active={encode(active.ToString())}";
        public string AnnouncementFetchAll(string token)
            => $"{BaseURL}/admin/announcement/all?token={encode(token)}";
        public string AnnouncementSetData(string token, SystemAnnouncementSummary summary)
            => $"{BaseURL}/admin/announcement/setData?token={encode(token)}&content={encode(JsonSerializer.Serialize(summary, SerializerOptions))}";
        public string AnnouncementSummary(string token)
            => $"{BaseURL}/admin/announcement/summary?token={encode(token)}";

        #endregion

        public string DumpSetData(string token, DataType type)
            => $"{BaseURL}/admin/data/setdata?token={encode(token)}&type={encode((int)type)}";
        public string DumpDataFetch(string token, DataType type)
            => $"{BaseURL}/admin/data/dump?token={encode(token)}&type={encode((int)type)}";

        #region User Management
        public string UserList(
            string token,
            string username = "",
            SearchMethod usernameSearchMethod = SearchMethod.Equals,
            long firstSeenTimestamp = 0,
            long lastSeenTimestamp = long.MaxValue)
            => $"{BaseURL}/admin/user/list?token={encode(token)}&username={encode(username)}&usernameSearchMethod={encode(usernameSearchMethod)}&firstSeenTimestamp={encode(firstSeenTimestamp)}&lastSeenTimestamp={lastSeenTimestamp}";
        public string UserPermissionGrant(
            string token,
            string username,
            AccountPermission permission)
            => $"{BaseURL}/admin/user/permission/grant?token={encode(token)}&username={encode(username)}&permission={encode((int)permission)}";
        public string UserPermissionRevoke(
            string token,
            string username,
            AccountPermission permission)
            => $"{BaseURL}/admin/user/permission/revoke?token={encode(token)}&username={encode(username)}&permission={encode((int)permission)}";

        public string UserGroupSet(
            string token)
            => $"{BaseURL}/admin/user/group/set?token={encode(token)}";

        #region User Disable/Enable
        public string UserDisable(
            string token,
            string username,
            string reason)
            => $"{BaseURL}/admin/user/disable?token={encode(token)}&username={encode(username)}&reason={encode(reason)}";
        public string UserPardon(
            string token,
            string username)
            => $"{BaseURL}/admin/user/pardon?token={encode(token)}&username={encode(username)}";
        #endregion

        #region Account License Management
        public string UserLicenseGrant(
            string token,
            string username,
            string license)
            => $"{BaseURL}/admin/user/license/grant?token={encode(token)}&username={encode(username)}&license={encode(license)}";
        public string UserLicenseRevoke(
            string token,
            string username,
            string license)
            => $"{BaseURL}/admin/user/license/revoke?token={encode(token)}&username={encode(username)}&license={encode(license)}";
        #endregion
        #endregion

        #region License Keys
        public string CreateLicenseKeys(
            string token)
            => $"{BaseURL}/admin/license/generateProductKey?token={encode(token)}";
        public string GetLicenseKeys(
            string token)
            => $"{BaseURL}/admin/license/getKeys?token={encode(token)}";
        public string GetLicenseKeys(
            string token,
            string remoteLocation)
            => $"{BaseURL}/admin/license/getKeys?token={encode(token)}&remoteLocation={encode(remoteLocation)}";
        public string LicenseKeyRedeem(
            string token,
            string LicenseKey)
            => $"{BaseURL}/license/redeem?token={encode(token)}&key={encode(LicenseKey)}";
        public string LicenseKeyDisable(
            string token,
            string keyId)
            => $"{BaseURL}/admin/license/disableKey?token={encode(token)}&keyId={encode(keyId)}";
        public string LicenseKeyEnable(
            string token,
            string keyId)
            => $"{BaseURL}/admin/license/enableKey?token={encode(token)}&keyId={encode(keyId)}";
        #endregion
        #endregion
    }
}
