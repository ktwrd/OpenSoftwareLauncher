using OSLCommon.Authorization;
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
            toolStripButtonRefresh.Text = LocaleManager.Get(toolStripButtonRefresh.Text);
            toolStripButtonBanTool.Text = LocaleManager.Get(toolStripButtonBanTool.Text);
            toolStripButtonPermissionTool.Text = LocaleManager.Get(toolStripButtonPermissionTool.Text);
            toolStripButtonGroupTool.Text = LocaleManager.Get(toolStripButtonGroupTool.Text);
            toolStripButtonGroupEditorTool.Text = LocaleManager.Get(toolStripButtonGroupEditorTool.Text);
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
                    var lvitem = new ListViewItem(new string[]
                    {
                        item.Username,
                        item.Enabled.ToString(),
                        item.Licenses.Length > 1 ? $"{item.Licenses.Length} " + LocaleManager.Get("License_Plural") : string.Join(", ", item.Licenses),
                        string.Join(", ", permissionString)
                    });
                    lvitem.Name = item.Username;
                    listViewAccounts.Items.Add(lvitem);
                }
            }
        }

        private void toolStripButtonRefresh_Click(object sender, EventArgs e)
        {
            Enabled = false;
            ReloadList(true);
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
    }
}
