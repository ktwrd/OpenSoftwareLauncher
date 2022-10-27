using kate.shared.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenSoftwareLauncher.DesktopWinForms
{
    public partial class AnnouncementEditDialog : Form
    {
        public AnnouncementEditDialog()
        {
            InitializeComponent();
        }
        public AnnouncementEditDialog(AnnouncementManagementForm manform)
        {
            InitializeComponent();
            Manager = manform;
            textBoxCode.Lines = Program.LocalContent.AnnouncementSummary.Entries[manform.targetIndex].Message.Split(new string[] { "\n" }, StringSplitOptions.None);
            checkBoxEnable.Checked = Program.LocalContent.AnnouncementSummary.Entries[manform.targetIndex].Active;
        }
        private AnnouncementManagementForm Manager;
        public void Locale()
        {
            Text = LocaleManager.Get("Title_AnnouncementEditDialog");
            checkBoxEnable.Text = LocaleManager.Get("EnableAnnouncement");
            buttonPush.Text = LocaleManager.Get("Push");
        }

        private void AnnouncementEditDialog_Shown(object sender, EventArgs e)
        {
            Locale();
        }

        private void timerTypeUpdate_Tick(object sender, EventArgs e)
        {
            if (textBoxCode.Text != webBrowserPreview.DocumentText)
                webBrowserPreview.DocumentText = textBoxCode.Text;
        }

        private void buttonPush_Click(object sender, EventArgs e)
        {
            Enabled = false;
            Manager.SetTargetContent(textBoxCode.Text, checkBoxEnable.Checked);
            Program.LocalContent.PushAnnouncements().Wait();
            Close();
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog()
            {
                Filter = "Any file|*.*",
                Multiselect = false
            };
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBoxCode.Lines = File.ReadAllLines(dialog.FileName);
                timerTypeUpdate_Tick(null, null);
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog()
            {
                Filter = "HTML file|*.*",
            };
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                File.WriteAllLines(dialog.FileName, textBoxCode.Lines);
                Process.Start("explorer", $"/select,\"{dialog.FileName}\"");
            }
        }
    }
}
