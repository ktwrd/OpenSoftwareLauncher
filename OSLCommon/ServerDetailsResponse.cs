using System;
using System.Collections.Generic;
using System.Text;

namespace OSLCommon
{
    public class ServerDetailsResponse
    {
        /// <summary>
        /// Measured in seconds since the server was started.
        /// </summary>
        public long Uptime { get; set; }
        public string Version { get; set; }
    }
}
