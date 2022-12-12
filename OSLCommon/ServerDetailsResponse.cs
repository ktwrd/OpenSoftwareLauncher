
using System.Collections.Generic;

namespace OSLCommon
{
    public class ServerDetailsResponse
    {
        /// <summary>
        /// Measured in seconds since the server was started.
        /// </summary>
        public long Uptime { get; set; }
        public string Version { get; set; }

        public string AuthProvider { get; set; }
        public string AuthProviderSignup { get; set; }
        public Features.Feature[] Features { get; set; }
    }
}
