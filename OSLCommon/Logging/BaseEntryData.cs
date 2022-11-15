using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OSLCommon.Logging
{
    public class BaseEntryData : IAuditEntryData
    {
        public BaseEntryData()
        {
            AuditType = AuditType.None;
        }
        [JsonIgnore]
        public virtual AuditType AuditType { get; set; }

        public virtual string SerializeToJSON()
        {
            var result = JsonSerializer.Serialize(this, new JsonSerializerOptions()
            {
                IgnoreReadOnlyFields = true,
                IgnoreReadOnlyProperties = true,
                IncludeFields = true
            });
            return result;
        }
    }
}
