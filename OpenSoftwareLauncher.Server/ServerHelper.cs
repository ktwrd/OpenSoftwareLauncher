using Microsoft.AspNetCore.Http;

namespace OpenSoftwareLauncher.Server
{
    public static class ServerHelper
    {
        public static string FindClientAddress(HttpContext context)
        {
            string address = context.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";

            string[] possibleHeaders = new string[]
            {
                "X-Forwarded-For",
                "X-Real-IP"
            };

            foreach (var item in possibleHeaders)
                if (context.Request.Headers.ContainsKey(item))
                {
                    address = context.Request.Headers[item];
                    break;
                }

            return address;
        }
    }
}
