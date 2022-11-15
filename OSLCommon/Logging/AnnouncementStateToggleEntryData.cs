using System;
using System.Collections.Generic;
using System.Text;

namespace OSLCommon.Logging
{
    public class AnnouncementStateToggleEntryData : BaseEntryData
    {
        public AnnouncementStateToggleEntryData()
            : base()
        {
            AuditType = AuditType.AnnouncementStateToggle;
            State = true;
        }
        public bool State { get; set; }
    }
}
