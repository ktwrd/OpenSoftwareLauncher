using System;
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

            toolStripButtonAuditLog.Text = LocaleManager.Get("Title_AuditLog");
            toolStripButtonAuditLog.ToolTipText = LocaleManager.Get("Title_AuditLog");

            toolStripButtonReleases.Text = LocaleManager.Get(toolStripButtonReleases.Text);
            toolStripButtonReleases.ToolTipText = toolStripButtonReleases.Text;

            Text = LocaleManager.Get(Text);
        }

        public UserManagementForm UserManagementForm;
        public LicenseManagmentForm LicenseManagmentForm;
        public AnnouncementManagementForm AnnouncementManagementForm;
        public LogForm LogForm;
        public AuditLogForm AuditLogForm;
        public ReleaseManagementForm ReleaseManagementForm;

        private void ParentForm_Shown(object sender, EventArgs e)
        {
            LogForm = new LogForm();
            LogForm.MdiParent = this;
            LogForm.Show();
            LogForm.WindowState = FormWindowState.Minimized;

            toolStripButtonUsers.Enabled = Program.Client.HasPermission(OSLCommon.Authorization.AccountPermission.USER_LIST);
            toolStripButtonAnnouncements.Enabled = Program.Client.HasPermission(OSLCommon.Authorization.AccountPermission.ANNOUNCEMENT_MANAGE);
            toolStripButtonLicenceManagement.Enabled = Program.Client.HasPermission(OSLCommon.Authorization.AccountPermission.LICENSE_MANAGE);
            toolStripButtonLicenseKeyCreator.Enabled = Program.Client.HasPermission(OSLCommon.Authorization.AccountPermission.LICENSE_MANAGE);
            toolStripButtonAuditLog.Enabled = Program.Client.HasPermission(OSLCommon.Authorization.AccountPermission.AUDITLOG_SELF)
                                           || Program.Client.HasPermission(OSLCommon.Authorization.AccountPermission.AUDITLOG_GLOBAL);
            toolStripButtonReleases.Enabled = Program.Client.HasPermission(OSLCommon.Authorization.AccountPermission.RELEASE_MANAGE);
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

        private void toolStripButtonAuditLog_Click(object sender, EventArgs e)
        {
            if (AuditLogForm == null || AuditLogForm.IsDisposed)
            {
                AuditLogForm = new AuditLogForm();
            }
            AuditLogForm.MdiParent = this;
            AuditLogForm.Show();
        }

        private void toolStripButtonReleases_Click(object sender, EventArgs e)
        {
            if (!Program.Client.HasPermission(OSLCommon.Authorization.AccountPermission.RELEASE_MANAGE)) return;
            if (ReleaseManagementForm == null || ReleaseManagementForm.IsDisposed)
            {
                ReleaseManagementForm = new ReleaseManagementForm();
            }
            ReleaseManagementForm.MdiParent = this;
            ReleaseManagementForm.Show();
        }
    }
}
