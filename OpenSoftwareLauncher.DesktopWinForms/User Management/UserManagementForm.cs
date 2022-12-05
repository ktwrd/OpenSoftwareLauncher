using OSLCommon.Authorization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenSoftwareLauncher.DesktopWinForms
{
    public partial class UserManagementForm : Form
    {
        public UserManagementForm()
        {
            InitializeComponent();
            Locale();
        }

        public void Locale()
        {
            Text = LocaleManager.Get(Text);

            toolStripButtonEdit.Text = LocaleManager.Get(toolStripButtonEdit.Text);
            toolStripButtonEdit.ToolTipText = toolStripButtonEdit.Text;

            toolStripButtonDelete.Text = LocaleManager.Get(toolStripButtonDelete.Text);
            toolStripButtonDelete.ToolTipText = toolStripButtonDelete.Text;

            toolStripButtonRefresh.Text = LocaleManager.Get(toolStripButtonRefresh.Text);
            toolStripButtonRefresh.ToolTipText = toolStripButtonRefresh.Text;
            
            toolStripButtonBanTool.Text = LocaleManager.Get(toolStripButtonBanTool.Text);
            toolStripButtonBanTool.ToolTipText = toolStripButtonBanTool.Text;

            toolStripButtonUnban.Text = LocaleManager.Get(toolStripButtonUnban.Text);
            toolStripButtonUnban.ToolTipText = toolStripButtonUnban.Text;

            toolStripButtonPermissionTool.Text = LocaleManager.Get(toolStripButtonPermissionTool.Text);
            toolStripButtonPermissionTool.ToolTipText = toolStripButtonPermissionTool.Text;
            
            toolStripButtonGroupTool.Text = LocaleManager.Get(toolStripButtonGroupTool.Text);
            toolStripButtonGroupTool.ToolTipText = toolStripButtonGroupTool.Text;

            toolStripButtonLicense.Text = LocaleManager.Get(toolStripButtonLicense.Text);
            toolStripButtonLicense.ToolTipText = toolStripButtonLicense.Text;

            toolStripButtonCreateServiceAccount.Text = LocaleManager.Get(toolStripButtonCreateServiceAccount.Text);
            toolStripButtonCreateServiceAccount.ToolTipText = toolStripButtonCreateServiceAccount.Text;

            toolStripButtonLicenseUtility.Text = LocaleManager.Get(toolStripButtonLicenseUtility.Text);
            toolStripButtonLicenseUtility.ToolTipText = toolStripButtonLicenseUtility.Text;

            toolStripButtonPermissionUtility.Text = LocaleManager.Get(toolStripButtonPermissionUtility.Text);
            toolStripButtonPermissionUtility.ToolTipText = toolStripButtonPermissionUtility.Text;


            foreach (ColumnHeader col in listViewAccounts.Columns)
            {
                col.Text = LocaleManager.Get(col.Text);
            }
        }

        public void ReloadList(bool pull=false)
        {
            listViewAccounts.Items.Clear();

            if (Program.Client == null) return;
            if (Program.Client.HasPermission(AccountPermission.USER_LIST))
            {
                Program.LocalContent.PullAccounts().Wait();

                foreach (var item in Program.LocalContent.AccountDetailList)
                {
                    var permissionString = new List<string>();
                    foreach (var p in item.Permissions)
                        permissionString.Add(p.ToString());

                    var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    long value = item.LastSeenTimestamp > 1 ? item.LastSeenTimestamp : 0;
                    DateTime dt = epoch;
                    try
                    {
                        dt = epoch.AddMilliseconds(value);
                    }
                    catch
                    {
                        Debugger.Break();
                    }
                    var lvitem = new ListViewItem(new string[]
                    {
                        item.Username,
                        item.Enabled.ToString(),
                        item.Licenses.Length > 1 ? $"{item.Licenses.Length} " + LocaleManager.Get("License_Plural") : string.Join(", ", item.Licenses),
                        string.Join(", ", permissionString),
                        value < 1 ? "Never" : dt.ToString()
                    });
                    if (!item.Enabled)
                        lvitem.ImageIndex = 1;
                    else if (item.ServiceAccount)
                        lvitem.ImageIndex = 0;
                    if (item.Enabled)
                        lvitem.Group = listViewAccounts.Groups[0];
                    else
                        lvitem.Group = listViewAccounts.Groups[1];
                    lvitem.Name = item.Username;
                    listViewAccounts.Items.Add(lvitem);
                }
            }
        }

        public void toolStripButtonRefresh_Click(object sender, EventArgs e)
        {
            Enabled = false;
            ReloadList(true);

            toolStripButtonCreateServiceAccount.Enabled = Program.Client.HasPermission(AccountPermission.SERVICEACCOUNT_MANAGE);
            toolStripButtonLicenseUtility.Enabled = Program.Client.HasPermission(AccountPermission.USER_LICENSE_MODIFY);
            toolStripButtonPermissionUtility.Enabled = Program.Client.HasPermission(AccountPermission.USER_PERMISSION_MODIFY);
            toolStripButtonDelete.Enabled = Program.Client.HasPermission(AccountPermission.USER_DELETE);

            Enabled = true;
        }

        private void UserManagementForm_Shown(object sender, EventArgs e)
        {
            toolStripButtonRefresh_Click(null, null);
        }
        public void Pull()
        {
            toolStripButtonRefresh_Click(null, null);
        }

        private void toolStripButtonPermissionTool_Click(object sender, EventArgs e)
        {
            if (SelectedAccounts.Length > 1 || SelectedAccounts.Length < 1) return;
            var form = new PermissionEditForm(SelectedAccounts[0]);
            form.MdiParent = this.MdiParent;
            form.Show();
        }

        public AccountDetailsResponse[] SelectedAccounts = Array.Empty<AccountDetailsResponse>();
        private void listViewAccounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewAccounts.SelectedItems.Count < 1)
            {
                SelectedAccounts = new AccountDetailsResponse[0];
            }
            else
            {
                var l = new List<AccountDetailsResponse>();
                foreach (ListViewItem item in listViewAccounts.SelectedItems)
                {
                    l = l.Concat(Program.LocalContent.AccountDetailList.Where(v => v.Username == item.Text)).ToList();
                }
                SelectedAccounts = l.ToArray();
            }

            toolStripButtonPermissionTool.Enabled = false;
            toolStripButtonBanTool.Enabled = false;
            toolStripButtonLicense.Enabled = false;
            toolStripButtonGroupTool.Enabled = false;
            toolStripButtonEdit.Enabled = false;
            toolStripButtonDelete.Enabled = false;
            if (SelectedAccounts.Length > 0)
            {
                if (SelectedAccounts.Length < 2)
                {
                    toolStripButtonPermissionTool.Enabled = true;
                    toolStripButtonBanTool.Enabled = true;
                    toolStripButtonLicense.Enabled = true;
                    toolStripButtonEdit.Enabled = true;
                }
                toolStripButtonGroupTool.Enabled = true;
                toolStripButtonUnban.Enabled = true;
                toolStripButtonDelete.Enabled = true;
            }
        }

        private async void toolStripButtonUnban_Click(object sender, EventArgs e)
        {
            var taskList = new List<Task>();
            var count = 0;
            foreach (var account in SelectedAccounts)
            {
                taskList.Add(new Task(delegate
                {
                    var result = Program.Client.AccountParson(account.Username).Result;
                    if (result != null)
                        MessageBox.Show(LocaleManager.Get(result.Message, @"Failed to ban user"));
                    else
                        count++;
                }));
            }
            foreach (var i in taskList)
                i.Start();
            await Task.WhenAll(taskList);
            ReloadList();

            MessageBox.Show($"Unbanned {count} Accounts");
        }

        private void toolStripButtonBanTool_Click(object sender, EventArgs e)
        {
            if (SelectedAccounts.Length > 1 || SelectedAccounts.Length < 1) return;
            var form = new UserDisableForm(SelectedAccounts[0]);
            form.MdiParent = MdiParent;
            form.Show();
        }

        private void toolStripButtonLicense_Click(object sender, EventArgs e)
        {
            if (SelectedAccounts.Length > 1 || SelectedAccounts.Length < 1) return;
            var form = new UserLicenseForm(SelectedAccounts[0]);
            form.MdiParent = MdiParent;
            form.Show();
        }

        private void listViewAccounts_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            listViewAccounts_SelectedIndexChanged(null, null);
        }

        private void listViewAccounts_MouseUp(object sender, MouseEventArgs e)
        {
            listViewAccounts_SelectedIndexChanged(null, null);
        }

        private void listViewAccounts_KeyUp(object sender, KeyEventArgs e)
        {
            // Ctrl+C
            if (e.KeyCode == Keys.C && e.Control)
            {
                List<string> usernames = new List<string>();
                foreach (ListViewItem item in listViewAccounts.SelectedItems)
                {
                    usernames.Add(item.Name);
                }
                var joined = string.Join("\n", usernames.ToArray());
                Clipboard.SetText(joined);
            }
        }

        private void toolStripButtonCreateServiceAccount_Click(object sender, EventArgs e)
        {
            Enabled = false;
            var form = new ServiceAccountCreateForm();
            form.MdiParent = MdiParent;
            form.FormClosed += (o, f) =>
            {
                toolStripButtonRefresh_Click(null, null);
                Enabled = true;
            };
            form.Show();
        }

        private async void toolStripButtonDelete_Click(object sender, EventArgs e)
        {
            var taskList = new List<Task>();
            var count = 0;
            foreach (var account in SelectedAccounts)
            {
                taskList.Add(new Task(delegate
                {
                    var result = Program.Client.AccountDelete(account.Username).Result;
                    if (result != null)
                        MessageBox.Show(LocaleManager.Get(result.Message, @"Failed to delete user"));
                    else
                        count++;
                }));
            }
            foreach (var i in taskList)
                i.Start();
            await Task.WhenAll(taskList);
            ReloadList();

            MessageBox.Show($"Deleted {count} Accounts");

        }

        public BulkUserLicenseForm BulkUserLicenseForm;

        private void toolStripButtonLicenseUtility_Click(object sender, EventArgs e)
        {
            if (!Program.Client.HasPermission(AccountPermission.USER_LICENSE_MODIFY)) return;
            if (BulkUserLicenseForm == null || BulkUserLicenseForm.IsDisposed)
            {
                BulkUserLicenseForm = new BulkUserLicenseForm();
            }
            BulkUserLicenseForm.MdiParent = MdiParent;
            BulkUserLicenseForm.Show();
            BulkUserLicenseForm.Focus();
        }
        public BulkUserPermissionForm BulkUserPermissionForm;
        private void toolStripButtonPermissionUtility_Click(object sender, EventArgs e)
        {
            if (!Program.Client.HasPermission(AccountPermission.USER_PERMISSION_MODIFY)) return;
            if (BulkUserPermissionForm == null || BulkUserPermissionForm.IsDisposed)
            {
                BulkUserPermissionForm = new BulkUserPermissionForm();
            }
            BulkUserPermissionForm.MdiParent = MdiParent;
            BulkUserPermissionForm.Show();
            BulkUserPermissionForm.Focus();
        }
    }
}
