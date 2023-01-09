using OSLCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenSoftwareLauncher.AdminClient
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
                listViewAnnouncement.Items.Add(listItem);
            }
            checkBoxEnableAnnouncements.Checked = Program.LocalContent.AnnouncementSummary.Active;
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
            if (listViewAnnouncement.SelectedItems.Count > 0)
                targetIndex = listViewAnnouncement.Items.IndexOf(listViewAnnouncement.SelectedItems[0]);
            else
                targetIndex = -1;
        }

        public int targetIndex = -1;

        public void SetTargetContent(string content, bool active)
        {
            Program.LocalContent.AnnouncementSummary.Entries[targetIndex].Message = content;
            Program.LocalContent.AnnouncementSummary.Entries[targetIndex].Active = active;
        }

        private void toolStripButtonAdd_Click(object sender, EventArgs e)
        {
            Enabled = false;
            var thing = new SystemAnnouncementEntry();
            Program.LocalContent.AnnouncementSummary.Entries.Add(thing);
            targetIndex = Program.LocalContent.AnnouncementSummary.Entries.IndexOf(thing);
            var form = new AnnouncementEditDialog(this);
            form.MdiParent = MdiParent;
            form.FormClosed += delegate
            {
                RedrawList();
                Enabled = true;
            };
            form.Show();
        }

        private void toolStripButtonEdit_Click(object sender, EventArgs e)
        {
            Enabled = false;
            var form = new AnnouncementEditDialog(this);
            form.MdiParent = MdiParent;
            form.FormClosed += delegate
            {
                RedrawList();
                Enabled = true;
            };
            form.Show();
        }

        private void toolStripButtonDelete_Click(object sender, EventArgs e)
        {
            Enabled = false;
            var newList = new List<SystemAnnouncementEntry>();
            int[] selectedIndexes = new int[listViewAnnouncement.SelectedItems.Count];
            for (int i = 0; i < listViewAnnouncement.SelectedItems.Count; i++)
                selectedIndexes[i] = listViewAnnouncement.Items.IndexOf(listViewAnnouncement.SelectedItems[i]);
            for (int i = 0; i < Program.LocalContent.AnnouncementSummary.Entries.Count; i++)
            {
                if (!selectedIndexes.Contains(i))
                    newList.Add(Program.LocalContent.AnnouncementSummary.Entries[i]);
            }
            Program.LocalContent.AnnouncementSummary.Entries = newList;
            Program.LocalContent.AnnouncementSummary.Active = checkBoxEnableAnnouncements.Checked;
            Program.LocalContent.PushAnnouncements().Wait();
            RedrawList();
            Enabled = true;
            MessageBox.Show($"Deleted {selectedIndexes.Length} items");
        }

        private void checkBoxEnableAnnouncements_CheckedChanged(object sender, EventArgs e)
        {
            Enabled = false;
            Program.LocalContent.AnnouncementSummary.Active = checkBoxEnableAnnouncements.Checked;
            Program.LocalContent.PushAnnouncements();
            Enabled = true;
        }
    }
}
