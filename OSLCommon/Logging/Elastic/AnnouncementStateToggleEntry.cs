using System;
using System.Collections.Generic;
using System.Text;

namespace OSLCommon.Logging.Elastic
{
    public class AnnouncementStateToggleEntry : BaseElasticEntry
    {
        public AnnouncementStateToggleEntry()
            : base()
        {
            State = false;
            AuditType = AuditType.AnnouncementStateToggle;
        }
        public AnnouncementStateToggleEntry(AnnouncementStateToggleEntryData data)
            : base()
        {
            State = data.State;
            AuditType = AuditType.AnnouncementStateToggle;
        }
        public bool State { get; set; }
        public AuditType AuditType { get; set; }
    }
}
