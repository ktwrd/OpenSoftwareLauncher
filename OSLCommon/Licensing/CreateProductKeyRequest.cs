using System;
using System.Collections.Generic;
using System.Text;

namespace OSLCommon.Licensing
{
    public class CreateProductKeyRequest
    {
        public int Count { get; set; }
        public string[] RemoteLocations { get; set; }
        public long ExpiryTimestamp { get; set; }
        public string Note { get; set; }
        public string GroupLabel { get; set; }
    }
}
