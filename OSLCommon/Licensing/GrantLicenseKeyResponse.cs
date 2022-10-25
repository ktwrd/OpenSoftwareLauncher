using System;
using System.Collections.Generic;
using System.Text;

namespace OSLCommon.Licensing
{
    public enum GrantLicenseKeyResponseCode
    {
        Invalid,
        AlreadyRedeemed,
        Granted
    }
    public class GrantLicenseKeyResponse
    {
        public GrantLicenseKeyResponseCode Code { get; set; }
        public string[] Products { get; set; }
    }
}
