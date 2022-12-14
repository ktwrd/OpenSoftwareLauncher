using OSLCommon.Licensing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.Json;

namespace OSLCommon.Logging
{
    public class LicenseDisableEntryData : BaseEntryData
    {
        public LicenseDisableEntryData()
            : base()
        {
            AuditType = AuditType.LicenseDisable;
            LicenseId = "";
            Metadata = new LicenseKeyMetadata();
        }
        public LicenseDisableEntryData(LicenseKeyMetadata key)
            : base()
        {
            AuditType = AuditType.LicenseDisable;
            LicenseId = key.UID;
            var options = new JsonSerializerOptions()
            {
                IgnoreReadOnlyFields = true,
                IgnoreReadOnlyProperties = true,
                IncludeFields = true
            };
            Metadata = JsonSerializer.Deserialize<LicenseKeyMetadata>(JsonSerializer.Serialize(key, options), options);
        }
        [Description("License ID")]
        public string LicenseId { get; set; }
        [Category("License Key")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public LicenseKeyMetadata Metadata { get; set; }
    }
}
