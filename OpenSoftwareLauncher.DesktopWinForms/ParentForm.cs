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
    public partial class ParentForm : Form
    {
        public ParentForm()
        {
            InitializeComponent();
            Locale();
        }

        public void Locale()
        {
            toolStripButtonUsers.Text = LocaleManager.Get(toolStripButtonUsers.Text);
            toolStripButtonUsers.ToolTipText = toolStripButtonUsers.Text;

            toolStripButtonAnnouncements.Text = LocaleManager.Get(toolStripButtonAnnouncements.Text);
            toolStripButtonAnnouncements.ToolTipText = toolStripButtonAnnouncements.Text;

            toolStripButtonLicenceManagement.Text = LocaleManager.Get(toolStripButtonLicenceManagement.Text);
            toolStripButtonLicenceManagement.ToolTipText = toolStripButtonLicenceManagement.Text;

            toolStripButtonLogs.Text = LocaleManager.Get(toolStripButtonLogs.Text);
            toolStripButtonLogs.ToolTipText = toolStripButtonLogs.Text;
            Text = LocaleManager.Get(Text);
        }

        public UserManagementForm UserManagementForm;
        public LicenseManagmentForm LicenseManagmentForm;
        public AnnouncementManagementForm AnnouncementManagementForm;

        private void ParentForm_Shown(object sender, EventArgs e)
        {
        }

        private void toolStripButtonUsers_Click(object sender, EventArgs e)
        {
            if (UserManagementForm == null || UserManagementForm.IsDisposed)
            {
                UserManagementForm = new UserManagementForm();
            }
            UserManagementForm.MdiParent = this;
            UserManagementForm.Show();
        }

        private void toolStripButtonLicenseKeyCreator_Click(object sender, EventArgs e)
        {
            var form = new LicenseKeyCreateDialog();
            form.MdiParent = this;
            form.Show();
        }


        private void toolStripButtonLicenceManagement_Click(object sender, EventArgs e)
        {
            if (LicenseManagmentForm == null || LicenseManagmentForm.IsDisposed)
            {
                LicenseManagmentForm = new LicenseManagmentForm();
            }
            LicenseManagmentForm.MdiParent = this;
            LicenseManagmentForm.Show();
        }

        private void toolStripButtonAnnouncements_Click(object sender, EventArgs e)
        {
            if (AnnouncementManagementForm == null || AnnouncementManagementForm.IsDisposed)
            {
                AnnouncementManagementForm = new AnnouncementManagementForm();
            }
            AnnouncementManagementForm.MdiParent = this;
            AnnouncementManagementForm.Show();
        }
    }
}
