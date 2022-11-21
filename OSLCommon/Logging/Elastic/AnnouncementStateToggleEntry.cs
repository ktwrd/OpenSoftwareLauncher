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
        }
        public AnnouncementStateToggleEntry(AnnouncementStateToggleEntryData data)
            : base()
        {
            State = data.State;
        }
        public bool State { get; set; }
    }
}
