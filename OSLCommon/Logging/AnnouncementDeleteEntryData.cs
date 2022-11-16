using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace OSLCommon.Logging
{
    public class AnnouncementDeleteEntryData : BaseEntryData
    {
        public AnnouncementDeleteEntryData()
            : base()
        {
            AuditType = AuditType.AnnouncementDelete;
            AnnouncementId = "";
            Entry = new SystemAnnouncementEntry();
        }
        public AnnouncementDeleteEntryData(SystemAnnouncementEntry entry)
            : base()
        {
            AuditType = AuditType.AnnouncementDelete;
            AnnouncementId = entry.ID;
            Entry = entry;
        }
        [Description("Announcement ID")]
        public string AnnouncementId { get; set; }
        [Category("Entry")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public SystemAnnouncementEntry Entry { get; set; }
    }
}
