using System.Net.Http;
using System.Net;
using System.Text.Json;

namespace OSLCommon.AuthProviders
{
    public class URLProvider : ITokenGranter
    {
        private class pLoginResponse
        {
            public string message { get; set; }
        }
        public string Name { get; set; }
        public HttpClient httpClient { get; private set; }
        public string Endpoint { get; private set; }
        private HttpClientHandler httpClientHandler;
        private CookieContainer Cookies
        {
            get { return httpClientHandler.CookieContainer; }
            set { httpClientHandler.CookieContainer = value; }
        }
        public bool Grant(string username, string password)
        {
            var url = $"{Endpoint}/api/login?email={WebUtility.UrlEncode(username)}&password={WebUtility.UrlEncode(password)}";
            var response = httpClient.GetAsync(url).Result;
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
        public URLProvider(string endpoint)
        {
            httpClientHandler = new HttpClientHandler
            {
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = new System.Net.CookieContainer()
            };
            httpClient = new HttpClient(httpClientHandler);

            Endpoint = endpoint;
            Name = "AuthProvider:URL";
        }
    }
}
