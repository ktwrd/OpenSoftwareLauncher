using kate.shared.Helpers;
using OSLCommon.Authorization;
using OSLCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenSoftwareLauncher.DesktopWinForms
{
    public static class Endpoint
    {
        public static string Base
        {
            get
            {
                return UserConfig.Connection_Endpoint;
            }
            set
            {
                UserConfig.Connection_Endpoint = value;
                UserConfig.Save();
                OnBaseAPIChanged(value);
            }
        }
        public static event StringDelegate BaseAPIChanged;
        public static void OnBaseAPIChanged(string route)
        {
            BaseAPIChanged?.Invoke(route);
        }
        private static string encode(string content)
            => WebUtility.UrlEncode(content);
        private static string encode(dynamic content)
            => encode(content.ToString());
        public static string ServerDetails
            => $"{Base}/server/details";
        public static string LatestReleaseBase
            => $"{Base}/release/latest";
        public static string LatestRelease(string id)
            => $"{LatestReleaseBase}/{id}";
        public static string LatestRelease(string id, string token)
            => $"{LatestReleaseBase}/{id}?token={encode(token)}";
        public static string Files(string hash)
            => $"{Base}/file?hash={encode(hash)}";
        public static string AvailableProducts()
            => $"{Base}/products/available";
        public static string AvailableProducts(string token)
            => $"{AvailableProducts()}?token={encode(token)}";

        public static string TokenGrant(string username, string password)
            => $"{Base}/token/grant?username={encode(username)}&password={encode(password)}";
        public static string TokenValidate(string token)
            => $"{Base}/token/validate?token={encode(token)}";
        public static string TokenDetails(string token)
            => $"{Base}/token/details?token={encode(token)}";
        public static string TokenRemove(string token, bool all = false)
            => $"{Base}/token/remove?token={encode(token)}&all={encode(all)}";

        public static string AccountDetails(string token)
            => $"{Base}/account?token={encode(token)}";

        public static string AnnouncementLatest()
            => $"{Base}/admin/announcement/latest";
        public static string AnnouncementCreate(string token, string content, bool active = true)
            => $"{Base}/admin/announcement/new?token={encode(token)}&content={encode(content)}&active={encode(active.ToString())}";
        public static string AnnouncementUpdateCurrent(string token, string content, bool active = true)
            => $"{Base}/admin/announcement/update?token={encode(token)}&content={encode(content)}&active={encode(active.ToString())}";
        public static string AnnouncementFetchAll(string token)
            => $"{Base}/admin/announcement/all?token={encode(token)}";
        public static string AnnouncementSetData(string token, SystemAnnouncementSummary summary)
            => $"{Base}/admin/announcement/setData?token={encode(token)}&content={encode(JsonSerializer.Serialize(summary, Program.serializerOptions))}";
        public static string AnnouncementSummary(string token)
            => $"{Base}/admin/announcement/summary?token={encode(token)}";

        public static string DumpSetData(string token, DataType type)
            => $"{Base}/admin/data/setdata?token={encode(token)}&type={encode((int)type)}";
        public static string DumpDataFetch(string token, DataType type)
            => $"{Base}/admin/data/dump?token={encode(token)}&type={encode((int)type)}";

        public static string UserList(
            string token,
            string username = "",
            SearchMethod usernameSearchMethod = SearchMethod.Equals,
            long firstSeenTimestamp = 0,
            long lastSeenTimestamp = long.MaxValue)
            => $"{Base}/admin/user/list?token={encode(token)}&username={encode(username)}&usernameSearchMethod={encode(usernameSearchMethod)}&firstSeenTimestamp={encode(firstSeenTimestamp)}&lastSeenTimestamp={lastSeenTimestamp}";
        public static string UserPermissionGrant(
            string token,
            string username,
            AccountPermission permission)
            => $"{Base}/admin/user/permission/grant?token={encode(token)}&username={encode(username)}&permission={encode((int)permission)}";
        public static string UserPermissionRevoke(
            string token,
            string username,
            AccountPermission permission)
            => $"{Base}/admin/user/permission/revoke?token={encode(token)}&username={encode(username)}&permission={encode((int)permission)}";

        public static string UserGroupSet(
            string token)
            => $"{Base}/admin/user/group/set?token={encode(token)}";

        public static string UserDisable(
            string token,
            string username,
            string reason)
            => $"{Base}/admin/user/disable?token={encode(token)}&username={encode(username)}&reason={encode(reason)}";
        public static string UserPardon(
            string token,
            string username)
            => $"{Base}/admin/user/pardon?token={encode(token)}&username={encode(username)}";

        #region Account License Management
        public static string UserLicenseGrant(
            string token,
            string username,
            string license)
            => $"{Base}/admin/user/license/grant?token={encode(token)}&username={encode(username)}&license={encode(license)}";
        public static string UserLicenseRevoke(
            string token,
            string username,
            string license)
            => $"{Base}/admin/user/license/revoke?token={encode(token)}&username={encode(username)}&license={encode(license)}";
        #endregion

        #region License Keys
        public static string CreateLicenseKeys(
            string token)
            => $"{Base}/admin/license/generateProductKey?token={encode(token)}";
        public static string GetLicenseKeys(
            string token)
            => $"{Base}/admin/license/getKeys?token={encode(token)}";
        public static string GetLicenseKeys(
            string token,
            string remoteLocation)
            => $"{Base}/admin/license/getKeys?token={encode(token)}&remoteLocation={encode(remoteLocation)}";
        public static string LicenseKeyRedeem(
            string token,
            string LicenseKey)
            => $"{Base}/license/redeem?token={encode(token)}&key={encode(LicenseKey)}";
        #endregion

    }
}
