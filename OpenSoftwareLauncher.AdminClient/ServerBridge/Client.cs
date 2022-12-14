using OSLCommon;
using OSLCommon.Authorization;
using OSLCommon.Licensing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using static OSLCommon.Licensing.AccountLicenseManager;

namespace OpenSoftwareLauncher.AdminClient.ServerBridge
{
    public class Client
    {
        internal string Username { get; set; } = "";
        internal string Token { get; set; } = "";
        internal AccountToken TokenData { get; set; } = null;
        internal AccountPermission[] Permissions { get; set; } = Array.Empty<AccountPermission>();
        public bool HasPermission(AccountPermission permission, string username=null, bool ignoreAdmin=false)
        {
            if (username == null)
            {
                if (!ignoreAdmin && Permissions.Contains(AccountPermission.ADMINISTRATOR))
                    return true;
                else if (Permissions.Contains(permission))
                    return true;
                else
                    return false;
            }
            else
            {
                foreach (var item in Program.LocalContent.AccountDetailList)
                {
                    if (item.Username == username)
                    {
                        if (!ignoreAdmin && item.Permissions.Contains(AccountPermission.ADMINISTRATOR))
                            return true;
                        else if (item.Permissions.Contains(permission))
                            return true;
                        else
                            return false;
                    }
                }
            }
            return false;
        }

        public ServerDetailsResponse ServerDetails { get; set; } = null;
        public AccountDetailsResponse AccountDetails { get; set; } = null;


        public Client()
        {
            HttpClient = new HttpClient();
            Auth = new AuthClient(this);
        }

        public HttpClient HttpClient;
        public AuthClient Auth;
        public void UpdateProperties()
        {
            var taskArray = new List<Task>()
            {
                new Task(delegate {FetchServerDetails();})
            };
            if (Token.Length > 1)
            {
                taskArray.Add(new Task(delegate { FetchAccountDetails(); }));
                taskArray.Add(new Task(delegate { Auth.ValidateToken(); }));
            }
            foreach (var i in taskArray)
                i.Start();
            Task.WhenAll(taskArray).Wait();
        }
        #region License Key
        public CreateLicenseKeyResponse CreateLicenseKeys(CreateProductKeyRequest content)
        {
            if (!HasPermission(AccountPermission.LICENSE_MANAGE))
                return new CreateLicenseKeyResponse
                {
                    Keys = Array.Empty<LicenseKeyMetadata>(),
                    GroupId = ""
                };
            var url = Endpoint.CreateLicenseKeys(Token);
            var response = HttpClient.PostAsync(url, new StringContent(JsonSerializer.Serialize(content, Program.serializerOptions))).Result;
            var stringContent = response.Content.ReadAsStringAsync().Result;

            switch ((int)response.StatusCode)
            {
                case 400:
                case 401:
                    var exceptionDeserialized = JsonSerializer.Deserialize<ObjectResponse<HttpException>>(stringContent, Program.serializerOptions);
                    Trace.WriteLine($"[Client->CreateLicenseKeys] Failed to create license keys\n--------\n{stringContent}\n--------\n");
                    new HttpExceptionModal(exceptionDeserialized.Data, (int)response.StatusCode, stringContent, url).Show();
                    return new CreateLicenseKeyResponse
                    {
                        Keys = Array.Empty<LicenseKeyMetadata>(),
                        GroupId = ""
                    };
                    break;
            }

            var deser = JsonSerializer.Deserialize<ObjectResponse<CreateLicenseKeyResponse>>(stringContent, Program.serializerOptions);
            Program.LocalContent.PullLicenseKeys().Wait();
            return deser.Data;
        }
        public async Task<LicenseKeyActionResult> EnableLicenseKey(string uid)
        {
            if (!HasPermission(AccountPermission.LICENSE_MANAGE))
                return LicenseKeyActionResult.Failure;

            var url = Endpoint.LicenseKeyEnable(Token, uid);
            var response = await HttpClient.GetAsync(url);
            var stringContent = response.Content.ReadAsStringAsync().Result;

            if ((int)response.StatusCode == 200)
            {
                var deser = JsonSerializer.Deserialize<ObjectResponse<LicenseKeyActionResult>>(stringContent, Program.serializerOptions);

                if (deser.Data == LicenseKeyActionResult.Success)
                    foreach (var item in Program.LocalContent.LicenseKeyList)
                        if (item.UID == uid)
                            item.Enable = true;
                return deser.Data;
            }
            return LicenseKeyActionResult.Failure;
        }
        public async Task<LicenseKeyActionResult> DisableLicenseKey(string uid)
        {
            if (!HasPermission(AccountPermission.LICENSE_MANAGE))
                return LicenseKeyActionResult.Failure;

            var url = Endpoint.LicenseKeyDisable(Token, uid);
            var response = await HttpClient.GetAsync(url);
            var stringContent = response.Content.ReadAsStringAsync().Result;

            if ((int)response.StatusCode == 200)
            {
                var deser = JsonSerializer.Deserialize<ObjectResponse<LicenseKeyActionResult>>(stringContent, Program.serializerOptions);

                if (deser.Data == LicenseKeyActionResult.Success)
                    foreach (var item in Program.LocalContent.LicenseKeyList)
                        if (item.UID == uid)
                            item.Enable = false;
                return deser.Data;
            }
            return LicenseKeyActionResult.Failure;
        }
        #endregion

        #region Permissions Management
        public async Task<HttpException> PermissionRevoke(string username, AccountPermission permission)
        {
            var url = Endpoint.UserPermissionRevoke(Token, username, permission);
            var response = HttpClient.GetAsync(url).Result;
            var stringContent = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var matches = Program.LocalContent.AccountDetailList.Where(v => v.Username == username);
                foreach (var i in matches)
                {
                    var l = new List<AccountPermission>(i.Permissions);
                    l.Remove(permission);

                    i.Permissions = l.ToArray();
                }
            }
            else
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound || response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var deser = JsonSerializer.Deserialize<ObjectResponse<HttpException>>(stringContent, Program.serializerOptions);
                    new HttpExceptionModal(deser.Data, (int)response.StatusCode, stringContent, url).Show();
                    return deser.Data;
                }
            }
            return null;
        }
        public async Task<HttpException> PermissionGrant(string username, AccountPermission permission)
        {
            var url = Endpoint.UserPermissionGrant(Token, username, permission);
            var response = HttpClient.GetAsync(url).Result;
            var stringContent = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var matches = Program.LocalContent.AccountDetailList.Where(v => v.Username == username);
                foreach (var i in matches)
                {
                    var l = new List<AccountPermission>(i.Permissions);
                    if (!l.Contains(permission))
                        l.Add(permission);

                    i.Permissions = l.ToArray();
                }
            }
            else
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound || response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var deser = JsonSerializer.Deserialize<ObjectResponse<HttpException>>(stringContent, Program.serializerOptions);
                    new HttpExceptionModal(deser.Data, (int)response.StatusCode, stringContent, url).Show();
                    return deser.Data;
                }
            }
            return null;
        }
        #endregion

        public async Task<HttpException> AccountBan(string username, string reason="No Reason")
        {
            var url = Endpoint.UserDisable(Token, username, reason);
            var response = HttpClient.GetAsync(url).Result;
            var stringContent = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var deser = JsonSerializer.Deserialize<ObjectResponse<AccountDetailsResponse>>(stringContent, Program.serializerOptions);
                foreach (var i in Program.LocalContent.AccountDetailList.ToArray())
                {
                    if (i.Username == username && i != deser.Data)
                    {
                        Program.LocalContent.AccountDetailList.Remove(i);
                        Program.LocalContent.AccountDetailList.Add(deser.Data);
                    }
                }
            }
            else
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound || response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var deser = JsonSerializer.Deserialize<ObjectResponse<HttpException>>(stringContent, Program.serializerOptions);
                    new HttpExceptionModal(deser.Data, (int)response.StatusCode, stringContent, url).Show();
                    return deser.Data;
                }
            }
            return null;
        }
        public async Task<HttpException> AccountPardon(string username)
        {
            var url = Endpoint.UserPardon(Token, username);
            var response = await HttpClient.GetAsync(url);
            var stringContent = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var deser = JsonSerializer.Deserialize<ObjectResponse<AccountDetailsResponse>>(stringContent, Program.serializerOptions);
                var t = Program.LocalContent.AccountDetailList.Where(v => v.Username != username).ToList();
                t.Add(deser.Data);
                Program.LocalContent.AccountDetailList = t;
            }
            else
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound || response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var deser = JsonSerializer.Deserialize<ObjectResponse<HttpException>>(stringContent, Program.serializerOptions);
                    new HttpExceptionModal(deser.Data, (int)response.StatusCode, stringContent, url).Show();
                    return deser.Data;
                }
            }
            return null;
        }
        public async Task<HttpException> AccountDelete(string username)
        {
            var url = Endpoint.UserDelete(Token, username);
            var response = await HttpClient.GetAsync(url);
            var stringContent = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Program.LocalContent.AccountDetailList =
                    Program.LocalContent.AccountDetailList.ToArray().Where(v => v.Username != username)
                    .ToList();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound || response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var deser = JsonSerializer.Deserialize<ObjectResponse<HttpException>>(stringContent, Program.serializerOptions);
                new HttpExceptionModal(deser.Data, (int)response.StatusCode, stringContent, url).Show();
                return deser.Data;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Nullable</returns>
        public ServerDetailsResponse FetchServerDetails()
        {
            ServerDetails = null;
            var targetURL = Endpoint.ServerDetails;

            var response = HttpClient.GetAsync(targetURL).Result;
            var stringContent = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Trace.WriteLine($"[AuthClient->FetchServerDetails] Invalid status code. Content\n================\n{stringContent}\n================\n");
                Program.MessageBoxShow(stringContent, LocaleManager.Get("ServerResponse_Invalid"));
                return null;
            }

            var deserialized = JsonSerializer.Deserialize<ServerDetailsResponse>(stringContent, Program.serializerOptions);
            ServerDetails = deserialized;
            return ServerDetails;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Nullable</returns>
        public AccountDetailsResponse FetchAccountDetails()
        {
            var targetURL = Endpoint.AccountDetails(Token);

            var response = HttpClient.GetAsync(targetURL).Result;
            var stringContent = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var exceptionDeserialized = JsonSerializer.Deserialize<ObjectResponse<HttpException>>(stringContent, Program.serializerOptions);
                Trace.WriteLine($"[AuthClient->FetchAccountDetails] Recieved HttpException. {exceptionDeserialized.Data.Message}");
                new HttpExceptionModal(exceptionDeserialized.Data, (int)response.StatusCode, stringContent, targetURL).Show();
                return null;
            }
            else if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Trace.WriteLine($"[AuthClient->FetchAccountDetails] Invalid status code. Content\n================\n{stringContent}\n================\n");
                Program.MessageBoxShow(stringContent, LocaleManager.Get("ServerResponse_Invalid"));
                return null;
            }

            var detailsDeserialized = JsonSerializer.Deserialize<ObjectResponse<AccountDetailsResponse>>(stringContent, Program.serializerOptions);
            if (detailsDeserialized == null)
            {
                Trace.WriteLine($"[AuthClient->FetchAccountDetails] Invalid body. Content\n================\n{stringContent}\n================\n");
                Program.MessageBoxShow(stringContent, LocaleManager.Get("ServerResponse_BodyDeserializationFailure"));
                return null;
            }
            AccountDetails = detailsDeserialized.Data;
            Permissions = detailsDeserialized.Data.Permissions;
            Program.Config.Auth.Username = AccountDetails.Username;
            Program.ConfigSave();
            return AccountDetails;
        }
    }
}
