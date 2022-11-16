using OSLCommon.Licensing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace OSLCommon.Logging
{
    public class LicenseRedeemEntryData : BaseEntryData
    {
        public LicenseRedeemEntryData()
            : base()
        {
            AuditType = AuditType.LicenseRedeem;
            LicenseId = "";
            Username = "";
        }
        public LicenseRedeemEntryData(LicenseKeyMetadata license)
            : base()
        {
            AuditType = AuditType.LicenseRedeem;
            LicenseId = license.UID;
            Username = license.ActivatedBy;
        }
        [Description("Username of the account that activated the license key")]
        public string Username { get; set; }
        [Description("ID of the License Key activated")]
        public string LicenseId { get; set; }
    }
}
