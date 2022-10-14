using OSLCommon;
using OSLCommon.Authorization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenSoftwareLauncher.DesktopWinForms.ServerBridge
{
    public class Client
    {
        internal string Username { get; set; } = "";
        internal string Token { get; set; } = "";
        internal AccountToken TokenData { get; set; } = null;
        internal AccountPermission[] Permissions { get; set; } = Array.Empty<AccountPermission>();
        public bool HasPermission(AccountPermission permission, bool ignoreAdmin=false)
        {
            if (!ignoreAdmin && Permissions.Contains(AccountPermission.ADMINISTRATOR))
                return true;
            else if (Permissions.Contains(permission))
                return true;
            else
                return false;
        }

        public ServerDetailsResponse ServerDetails { get; set; } = null;
        public AccountDetailsResponse AccountDetails { get; set; } = null;


        public Client()
        {
            HttpClient = new HttpClient();
        }

        public HttpClient HttpClient;

        #region ValidateCredentials
        public void UpdateProperties()
        {
            var taskArray = new List<Task>()
            {
                new Task(delegate {FetchServerDetails();})
            };
            if (Token.Length > 1)
            {
                taskArray.Add(new Task(delegate { FetchAccountDetails(); }));
                taskArray.Add(new Task(delegate { ValidateToken(); }));
            }
            foreach (var i in taskArray)
                i.Start();
            Task.WhenAll(taskArray).Wait();
        }


        public GrantTokenResponse ValidateCredentials(string username, string password, string endpoint)
        {
            UserConfig.Connection_Endpoint = endpoint;
            Endpoint.Base = endpoint;
            var response = HttpClient.GetAsync(Endpoint.TokenGrant(username, password)).Result;
            var stringContent = response.Content.ReadAsStringAsync().Result;

            var deserialized = JsonSerializer.Deserialize<ObjectResponse<GrantTokenResponse>>(stringContent, Program.serializerOptions);
            
            if (deserialized.Data.Success)
            {
                TokenData = deserialized.Data.Token;
                Token = TokenData.Token;
                UserConfig.Auth_Token = TokenData.Token;
                UserConfig.Save();
                Permissions = deserialized.Data.Permissions;
            }

            Trace.WriteLine($"[Client->ValidateCredentials] Recieved response \"{deserialized.Data.Message}\"");

            return deserialized.Data;
        }
        public GrantTokenResponse ValidateCredentials(string username, string password)
            => ValidateCredentials(username, password, UserConfig.GetString("Connection", "Endpoint"));
        #endregion

        #region ValidateToken
        public AccountTokenDetailsResponse ValidateToken(string token, string endpoint, bool save=true, bool messageBox=false)
        {
            UserConfig.Connection_Endpoint = endpoint;
            var url = Endpoint.TokenDetails(token);
            var response = HttpClient.GetAsync(url).Result;
            var stringCon = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var deserialized = JsonSerializer.Deserialize<ObjectResponse<AccountTokenDetailsResponse>>(stringCon, Program.serializerOptions);
                if (save)
                {
                    TokenData = new AccountToken(deserialized.Data)
                    {
                        Token = token
                    };
                    Token = TokenData.Token;
                    UserConfig.Auth_Token = token;
                    UserConfig.Save();
                }
                return deserialized.Data;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var exceptionDeserialized = JsonSerializer.Deserialize<ObjectResponse<HttpException>>(stringCon, Program.serializerOptions);
                if (messageBox)
                {
                    MessageBox.Show(LocaleManager.Get("Client_TokenGrantFailed") + "\n\n" + LocaleManager.Get(exceptionDeserialized.Data.Message), LocaleManager.Get("Client_TokenGrantFailed_Title"));
                }
            }
            Program.MessageBoxShow(stringCon, LocaleManager.Get("ServerResponse_Invalid"));
            return null;
        }
        public AccountTokenDetailsResponse ValidateToken(string endpoint, bool save = true, bool messageBox = false)
            => ValidateToken(UserConfig.Auth_Token, endpoint, save, messageBox);
        public AccountTokenDetailsResponse ValidateToken(bool save = true, bool messageBox = false)
            => ValidateToken(UserConfig.Connection_Endpoint, save, messageBox);
        #endregion

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
            var dynamicDeserialized = JsonSerializer.Deserialize<ObjectResponse<dynamic>>(stringContent, Program.serializerOptions);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var exceptionDeserialized = JsonSerializer.Deserialize<ObjectResponse<HttpException>>(stringContent, Program.serializerOptions);
                Trace.WriteLine($"[AuthClient->FetchAccountDetails] Recieved HttpException. {exceptionDeserialized.Data.Message}");
                Program.MessageBoxShow(LocaleManager.Get(exceptionDeserialized.Data.Message), LocaleManager.Get("ServerResponse_HttpException"));
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
            return AccountDetails;
        }
    }
}
