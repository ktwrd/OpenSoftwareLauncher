using System;
using System.Collections.Generic;
using System.Text;

namespace OSLCommon.Licensing
{
    public class AccountLicenseManagerSummary
    {
        public LicenseKeyMetadata[] Keys = Array.Empty<LicenseKeyMetadata>();
        public LicenseGroup[] Groups = Array.Empty<LicenseGroup>();
    }
}
