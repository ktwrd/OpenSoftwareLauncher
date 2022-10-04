using OSLCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSoftwareLauncher.DesktopWinForms.ServerBridge
{
    public class SystemAnnouncementSummaryAsList : SystemAnnouncementSummary
    {
        public new List<SystemAnnouncementEntry> Entries { get; set; }
        public SystemAnnouncementSummaryAsList()
            : base()
        {
            Entries = new List<SystemAnnouncementEntry>();
        }
    }
}
