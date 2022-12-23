using System.Net.Http;

namespace OSLCommon.AuthProviders
{
    public interface ITokenGranter
    {
        HttpClient httpClient { get; }
        bool Grant(string username, string password);
        string Name { get; set; }
    }
}
