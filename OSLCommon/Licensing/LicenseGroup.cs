using System;
using System.Collections.Generic;
using System.Text;

namespace OSLCommon.Licensing
{
    public class LicenseGroup
    {
        public string UID { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string[] LicenseIds { get; set; } = Array.Empty<string>();
        public long CreatedTimestamp { get; set; } = 0;
        public string CreatedBy { get; set; } = "";
        public string Note { get; set; } = "";
    }
}
