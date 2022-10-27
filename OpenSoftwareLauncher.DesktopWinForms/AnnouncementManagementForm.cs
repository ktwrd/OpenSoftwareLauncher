using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenSoftwareLauncher.DesktopWinForms
{
    public partial class AnnouncementManagementForm : Form
    {
        public AnnouncementManagementForm()
        {
            InitializeComponent();
        }
        public void Locale()
        {
            checkBoxEnableAnnouncements.Text = LocaleManager.Get("EnableAnnouncement_Plural");
            toolStripButtonAdd.Text = LocaleManager.Get("Add");
            toolStripButtonAdd.ToolTipText = toolStripButtonAdd.Text;
            toolStripButtonEdit.Text = LocaleManager.Get("Edit");
            toolStripButtonEdit.ToolTipText = toolStripButtonEdit.Text;
            toolStripButtonDelete.Text = LocaleManager.Get("Delete");
            toolStripButtonDelete.ToolTipText = toolStripButtonDelete.Text;
            Text = LocaleManager.Get("Title_AnnouncementManagementForm");
        }

        public void Refresh(bool pull = false)
        {
            if (pull)
                Program.LocalContent.PullAnnouncements().Wait();

            RedrawList();
            checkBoxEnableAnnouncements.Checked = Program.LocalContent.AnnouncementSummary.Active;
        }
        public void RedrawList()
        {
            listViewAnnouncement.Items.Clear();
            foreach (var item in Program.LocalContent.AnnouncementSummary.Entries)
            {
                var listItem = new ListViewItem(new string[]
                {
                    item.Message,
                    Program.Epoch.AddMilliseconds(item.Timestamp).ToLocalTime().ToString()
                });
                if (item.Active)
                    listItem.ImageIndex = 0;
            }
        }

        private void AnnouncementManagementForm_Shown(object sender, EventArgs e)
        {
            Locale();
            Refresh(true);
        }

        private void listViewAnnouncement_SelectedIndexChanged(object sender, EventArgs e)
        {
            toolStripButtonDelete.Enabled = listViewAnnouncement.SelectedItems.Count > 0;
            toolStripButtonEdit.Enabled = listViewAnnouncement.SelectedItems.Count > 0 && listViewAnnouncement.SelectedItems.Count < 2;
        }
    }
}
