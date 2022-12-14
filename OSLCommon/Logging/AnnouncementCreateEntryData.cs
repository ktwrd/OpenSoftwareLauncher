using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.Json;

namespace OSLCommon.Logging
{
    public class AnnouncementCreateEntryData : BaseEntryData
    {
        public AnnouncementCreateEntryData()
            : base()
        {
            AuditType = AuditType.AnnouncementCreate;
            Id = "";
            Entry = new SystemAnnouncementEntry();
        }
        public AnnouncementCreateEntryData(SystemAnnouncementEntry entry)
             : base()
        {
            AuditType = AuditType.AnnouncementCreate;
            Id = entry.ID;
            var options = new JsonSerializerOptions
            {
                IgnoreReadOnlyFields = true,
                IgnoreReadOnlyProperties = true,
                IncludeFields = true
            };
            Entry = JsonSerializer.Deserialize<SystemAnnouncementEntry>(JsonSerializer.Serialize(entry, options), options);
        }
        [Description("Announcement ID")]
        public string Id { get; set; }
        [Category("Entry")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public SystemAnnouncementEntry Entry { get; set; }

    }
}
