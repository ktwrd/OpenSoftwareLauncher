using System;
using System.Collections.Generic;
using System.Text;

namespace OSLCommon.Licensing
{
    public class CreateLicenseKeyResponse
    {
        public LicenseKeyMetadata[] Keys { get; set; }
        public string GroupId { get; set; }
    }
}
