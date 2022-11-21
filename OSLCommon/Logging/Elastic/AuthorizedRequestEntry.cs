using System;
using System.Collections.Generic;
using System.Text;

namespace OSLCommon.Logging.Elastic
{
    public class AuthorizedRequestEntry : BaseElasticEntry
    {
        public AuthorizedRequestEntry()
            : base()
        {
            Path = "/";
            UserAgent = "unknown";
            Address = "0.0.0.0";
            Method = "GET";
        }
        public string Path { get; set; }
        public string UserAgent { get; set; }
        public string Address { get; set; }
        public string Method { get; set; }
    }
}
