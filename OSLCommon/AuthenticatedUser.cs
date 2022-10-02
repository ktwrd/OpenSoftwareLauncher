using OSLCommon.Minalyze;
using kate.shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OSLCommon
{
    public class AuthenticatedUser
    {
        public static JsonSerializerOptions serializerOptions = new JsonSerializerOptions()
        {
            IgnoreReadOnlyFields = false,
            IgnoreReadOnlyProperties = false,
            IncludeFields = true
        };

        public bool IsAdmin { get; private set; }
        private List<string> availableServices = new List<string>();
        public string AvailableServices => string.Join(" ", availableServices.ToArray());

        private HttpClient httpClient;
        private HttpClientHandler httpClientHandler;
        private string credentialHash = "";
        private string name = "";
        private string username = "";
        private string password = "";
        public string Username => username;
        public string Name => name;
        public string Token { get; private set; }
        private void recalculateCredentialHash()
        {
            SHA256 sha256Instance = SHA256.Create();
            var computedHash = sha256Instance.ComputeHash(Encoding.UTF8.GetBytes($"{username}{password}"));
            credentialHash = GeneralHelper.Base62Encode(computedHash);
        }
        public AuthenticatedUser(string username, string password)
        {
            this.username = username;
            this.password = password;
            Token = GeneralHelper.GenerateToken(32);
            recalculateCredentialHash();

            httpClientHandler = new HttpClientHandler
            {
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = new System.Net.CookieContainer()
            };
            httpClient = new HttpClient(httpClientHandler);

            RefreshFlags();
        }
        private CookieContainer Cookies
        {
            get { return httpClientHandler.CookieContainer; }
            set { httpClientHandler.CookieContainer = value; }
        }
        public MinaloggerTokenGrant tokenGranter = new MinaloggerTokenGrant();
        public bool IsCredentialsValid()
        {
            var isvalid = tokenGranter.Grant(Username, password);
            if (isvalid)
            {
                httpClient.GetAsync($"https://minalogger.com/api/login?email={WebUtility.UrlEncode(username)}&password={WebUtility.UrlEncode(password)}").Wait();
            }
            return isvalid;
        }
        public void RefreshFlags()
        {
            var taskList = new List<Task>();
            var actionList = new List<Action>();

            availableServices.Clear();

            var cnt = IsCredentialsValid();
            if (!cnt) return;

            // Check admin
            actionList.Add(new Action(delegate
            {
                var response = httpClient.GetAsync($"https://minalogger.com/api/IsUserAdmin").Result;
                if ((int)response.StatusCode < 200 || (int)response.StatusCode > 299) return;
                var stringContent = response.Content.ReadAsStringAsync().Result;
                var deserialized = JsonSerializer.Deserialize<resIsAdmin>(stringContent, serializerOptions);
                if (deserialized != null)
                    IsAdmin = deserialized.isAdmin;
                else
                    IsAdmin = false;
            }));

            // Check if the user has the MINALOGGER_DESKTOP subscription
            actionList.Add(new Action(delegate
            {
                var response = httpClient.GetAsync($"https://minalogger.com/api/hasSubscription?tool_id=4").Result;
                if ((int)response.StatusCode < 200 || (int)response.StatusCode > 299) return;
                var stringContent = response.Content.ReadAsStringAsync().Result;
                var deserialized = JsonSerializer.Deserialize<resIsSubscribed>(stringContent, serializerOptions);
                if (deserialized != null && deserialized.isSubscribed)
                    availableServices.Add("ml2");
            }));

            // Check Geolog
            actionList.Add(new Action(delegate
            {
                var response = httpClient.GetAsync("https://minalogger.com/api/IsUserSubscribedToGeoLog").Result;
                if ((int)response.StatusCode < 200 || (int)response.StatusCode > 299) return;
                var stringContent = response.Content.ReadAsStringAsync().Result;
                var deserialized = JsonSerializer.Deserialize<resIsSubscribed>(stringContent, serializerOptions);
                if (deserialized != null && deserialized.isSubscribed)
                    availableServices.Add("geolog");
            }));

            // Check GeoTech
            actionList.Add(new Action(delegate
            {
                var response = httpClient.GetAsync("https://minalogger.com/api/IsUserSubscribedToGeoTech").Result;
                if ((int)response.StatusCode < 200 || (int)response.StatusCode > 299) return;
                var stringContent = response.Content.ReadAsStringAsync().Result;
                var deserialized = JsonSerializer.Deserialize<resIsSubscribed>(stringContent, serializerOptions);
                if (deserialized != null && deserialized.isSubscribed)
                    availableServices.Add("geotech");
            }));

            foreach (var i in actionList)
                taskList.Add(Task.Run(i));
            Task.WhenAll(taskList.ToArray()).Wait();
        }
        #region Validation
        public bool ValidateHash(string hash)
        {
            recalculateCredentialHash();
            return credentialHash == hash;
        }
        public bool ValidateToken(string token)
        {
            recalculateCredentialHash();
            return Token == token;
        }
        #endregion

        #region Private Classes
        private class resIsAdmin
        {
            public bool isAdmin = false;
        }
        private class resIsSubscribed
        {
            public bool isSubscribed = false;
        }
        #endregion
    }
}
