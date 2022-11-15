using OSLCommon.Licensing;
using System;
using System.Collections.Generic;
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
        public string LicenseId { get; set; }
        public string Username { get; set; }
    }
}
