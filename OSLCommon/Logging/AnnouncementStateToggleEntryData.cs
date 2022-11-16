using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        [Description("Are announcements enabled?")]
        public bool State { get; set; }
    }
}
