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
            toolStripButtonEdit.Text = LocaleManager.Get("Edit");
            toolStripButtonDelete.Text = LocaleManager.Get("Delete");
            Text = LocaleManager.Get("Title_AnnouncementManagementForm");
        }

        private void AnnouncementManagementForm_Shown(object sender, EventArgs e)
        {
            Locale();
        }
    }
}
