using OSLCommon;
using OSLCommon.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenSoftwareLauncher.DesktopWinForms
{
    public class Client
    {
        internal string Username { get; set; } = "";
        internal string Token { get; set; }
        internal AccountToken TokenData { get; set; } = null;
        internal AccountPermission[] Permissions { get; set; } = Array.Empty<AccountPermission>();

        public Client()
        {
            HttpClient = new HttpClient();
        }

        public HttpClient HttpClient;

        public GrantTokenResponse ValidateCredentials(string username, string password, string endpoint)
        {
            UserConfig.Connection_Endpoint = endpoint;
            var response = HttpClient.GetAsync(Endpoint.TokenGrant(username, password)).Result;
            var stringContent = response.Content.ReadAsStringAsync().Result;

            var deserialized = JsonSerializer.Deserialize<ObjectResponse<GrantTokenResponse>>(stringContent, Program.serializerOptions);
            
            if (deserialized.Data.Success)
            {
                TokenData = deserialized.Data.Token;
                Permissions = deserialized.Data.Permissions;
            }

            return deserialized.Data;
        }

        public AccountTokenDetailsResponse ValidateToken(string token, string endpoint)
        {
            UserConfig.Connection_Endpoint = endpoint;
            var response = HttpClient.GetAsync(Endpoint.TokenDetails(token)).Result;
            var stringCon = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var deserialized = JsonSerializer.Deserialize<ObjectResponse<AccountTokenDetailsResponse>>(stringCon, Program.serializerOptions);
                return deserialized.Data;
            }
            MessageBox.Show(stringCon, $"Invalid Response from Server");
            return null;

        }
    }
}
