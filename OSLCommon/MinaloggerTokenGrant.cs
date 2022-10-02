using OSLCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace OSLCommon.Minalyze
{
    public class MinaloggerTokenGrant : ITokenGranter
    {
        private class pLoginResponse
        {
            public string message { get; set; }
        }
        public HttpClient httpClient { get; private set; }
        private HttpClientHandler httpClientHandler;
        private CookieContainer Cookies
        {
            get { return httpClientHandler.CookieContainer; }
            set { httpClientHandler.CookieContainer = value; }
        }
        public bool Grant(string username, string password)
        {
            var response = httpClient.GetAsync($"https://minalogger.com/api/login?email={WebUtility.UrlEncode(username)}&password={WebUtility.UrlEncode(password)}").Result;
            var stringContent = response.Content.ReadAsStringAsync().Result;

            pLoginResponse loginResponse = JsonSerializer.Deserialize<pLoginResponse>(stringContent, new JsonSerializerOptions()
            {
                IgnoreReadOnlyFields = false,
                IgnoreReadOnlyProperties = false,
                IncludeFields = true
            });
            if (loginResponse == null || response.StatusCode == (HttpStatusCode)401)
            {
                return false;
            }
            return loginResponse.message == "success";
        }
        public MinaloggerTokenGrant()
        {
            httpClientHandler = new HttpClientHandler
            {
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = new System.Net.CookieContainer()
            };
            httpClient = new HttpClient(httpClientHandler);
        }
    }
}
