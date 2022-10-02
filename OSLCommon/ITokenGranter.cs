using System.Net.Http;

namespace OSLCommon
{
    public interface ITokenGranter
    {
        HttpClient httpClient { get; }
        bool Grant(string username, string password);
    }
}
