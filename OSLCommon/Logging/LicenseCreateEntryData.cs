using OSLCommon.Licensing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace OSLCommon.Logging
{
    public class LicenseCreateEntryData : BaseEntryData
    {
        public LicenseCreateEntryData()
            : base()
        {
            AuditType = AuditType.LicenseCreate;
            LicenseId = "";
            Metadata = new LicenseKeyMetadata();
        }
        public LicenseCreateEntryData(LicenseKeyMetadata key)
            : base()
        {
            AuditType = AuditType.LicenseCreate;
            LicenseId = key.UID;
            var options = new JsonSerializerOptions()
            {
                IgnoreReadOnlyFields = true,
                IgnoreReadOnlyProperties = true,
                IncludeFields = true
            };
            Metadata = JsonSerializer.Deserialize<LicenseKeyMetadata>(JsonSerializer.Serialize(key, options), options);
        }

        public string LicenseId { get; set; }
        public LicenseKeyMetadata Metadata { get; set; }
    }
}
