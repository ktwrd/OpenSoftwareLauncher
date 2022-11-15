using System;
using System.Collections.Generic;
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
        public string AnnouncementId { get; set; }
        public SystemAnnouncementEntry Entry { get; set; }
    }
}
