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
    public partial class BulkUserPermissionForm : Form
    {
        public BulkUserPermissionForm()
        {
            InitializeComponent();
        }

        #region Selection Buttons
        private void buttonAccountGrant_SelectAll_Click(object sender, EventArgs e)
        {
            FormHelper.SelectAll(checkedListBoxAccountGrant);
        }
        private void buttonAccountGrant_SelectInvert_Click(object sender, EventArgs e)
        {
            FormHelper.SelectInverse(checkedListBoxAccountGrant);
        }
        private void buttonPermissionGrant_SelectAll_Click(object sender, EventArgs e)
        {
            FormHelper.SelectAll(checkedListBoxPermissionGrant);
        }
        private void buttonPermissionGrant_SelectInvert_Click(object sender, EventArgs e)
        {
            FormHelper.SelectInverse(checkedListBoxPermissionGrant);
        }
        private void buttonAccountRevoke_SelectAll_Click(object sender, EventArgs e)
        {
            FormHelper.SelectAll(checkedListBoxAccountRevoke);
        }
        private void buttonAccountRevoke_SelectInvert_Click(object sender, EventArgs e)
        {
            FormHelper.SelectInverse(checkedListBoxAccountRevoke);
        }
        private void buttonPermissionRevoke_SelectAll_Click(object sender, EventArgs e)
        {
            FormHelper.SelectAll(checkedListBoxPermissionRevoke);
        }
        private void buttonPermissionRevoke_SelectInvert_Click(object sender, EventArgs e)
        {
            FormHelper.SelectInverse(checkedListBoxPermissionRevoke);
        }
        #endregion

        private void BulkUserPermissionForm_Shown(object sender, EventArgs e)
        {
            Text = LocaleManager.Get(Text);
            FormHelper.LocaleControl(this);
            Pull();
        }

        public async Task Pull()
        {
            Enabled = false;
            await Program.LocalContent.PullAccounts();
            ReloadLists();
            Enabled = true;
        }
        public void ReloadLists()
        {
            ReloadGrantList();
            ReloadRevokeList();
        }
        #region Revoke List Reload
        public void ReloadRevokeList()
        {
            ReloadRevokeListUsernames();
            ReloadRevokeListLicenses();
        }
        public void ReloadRevokeListUsernames()
            => FormHelper.ReloadGenericListUsernames(checkedListBoxAccountRevoke);
        public void ReloadRevokeListLicenses()
            => FormHelper.ReloadGenericListPermissions(checkedListBoxPermissionRevoke);
        #endregion
        #region Grant List Reload
        public void ReloadGrantList()
        {
            ReloadGrantListUsernames();
            ReloadGrantListLicenses();
        }
        public void ReloadGrantListUsernames()
        {
            FormHelper.ReloadGenericListUsernames(checkedListBoxAccountGrant);
        }
        public void ReloadGrantListLicenses()
        {
            FormHelper.ReloadGenericListPermissions(checkedListBoxPermissionGrant);
        }
        #endregion

        private Dictionary<string, AccountPermission[]> GenerateAccountPermissionPairs(CheckedListBox userList, CheckedListBox permissionBox)
        {
            var accountDictionary = new Dictionary<string, AccountPermission[]>();

            var selectedAccounts = userList.CheckedItems.Cast<string>();
            var selectedPermissions = permissionBox.CheckedItems.Cast<string>().Select(v => (AccountPermission)Enum.Parse(typeof(AccountPermission), v));
            foreach (var account in Program.LocalContent.AccountDetailList)
            {
                if (selectedAccounts.Contains(account.Username))
                {
                    var licenseList = new List<AccountPermission>();
                    foreach (var item in selectedPermissions)
                        if (!account.Permissions.Contains(item))
                            licenseList.Add(item);
                    accountDictionary.Add(account.Username, licenseList.ToArray());
                }
            }

            return accountDictionary;
        }
        private Dictionary<string, AccountPermission[]> GenerateAccountPermissionPairsRevoke(CheckedListBox userList, CheckedListBox permissionBox)
        {
            var accountDictionary = new Dictionary<string, AccountPermission[]>();
            var selectedAccounts = userList.CheckedItems.Cast<string>();
            var selectedLicenses = permissionBox.CheckedItems.Cast<string>().Select(v => (AccountPermission)Enum.Parse(typeof(AccountPermission), v));
            foreach (var account in Program.LocalContent.AccountDetailList)
            {
                if (selectedAccounts.Contains(account.Username))
                {
                    var licenseList = new List<AccountPermission>();
                    foreach (var item in selectedLicenses)
                        if (account.Permissions.Contains(item))
                            licenseList.Add(item);
                    accountDictionary.Add(account.Username, licenseList.ToArray());
                }
            }
            return accountDictionary;
        }

        public async Task Push_Grant(bool pullAccounts = true)
        {
            var accountDictionary = GenerateAccountPermissionPairs(checkedListBoxAccountGrant, checkedListBoxPermissionGrant);

            var taskList = new List<Task>();
            foreach (var pair in accountDictionary)
            {
                foreach (var permission in pair.Value)
                {
                    taskList.Add(new Task(delegate
                    {
                        var res = Program.Client.HttpClient.GetAsync(Endpoint.UserPermissionGrant(Program.Client.Token, pair.Key, permission)).Result;
                        var stringcontent = res.Content.ReadAsStringAsync().Result;
                        if (res.StatusCode != System.Net.HttpStatusCode.OK)
                        {
#if DEBUG
                            Debugger.Break();
#endif
                        }
                    }));
                }
            }

            foreach (var i in taskList)
                i.Start();
            await Task.WhenAll(taskList);
            if (pullAccounts)
                await Program.LocalContent.PullAccounts();
        }
        public async Task Push_Revoke(bool pullAccounts = true)
        {
            var accountDictionary = GenerateAccountPermissionPairsRevoke(checkedListBoxAccountRevoke, checkedListBoxPermissionRevoke);

            var taskList = new List<Task>();
            foreach (var pair in accountDictionary)
            {
                foreach (var permission in pair.Value)
                {
                    taskList.Add(new Task(delegate
                    {
                        var res = Program.Client.HttpClient.GetAsync(Endpoint.UserPermissionRevoke(Program.Client.Token, pair.Key, permission)).Result;
                        var stringcontent = res.Content.ReadAsStringAsync().Result;
                        if (res.StatusCode != System.Net.HttpStatusCode.OK)
                        {
#if DEBUG
                            Debugger.Break();
#endif
                        }
                    }));
                }
            }

            foreach (var i in taskList)
                i.Start();
            await Task.WhenAll(taskList);
            if (pullAccounts)
                await Program.LocalContent.PullAccounts();
        }

        private void BulkUserPermissionForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Program.ClientContext.ParentForm != null && Program.ClientContext.ParentForm.UserManagementForm != null)
                Program.ClientContext.ParentForm.UserManagementForm.toolStripButtonRefresh_Click(null, null);
        }

        private async void buttonGrantPush_Click(object sender, EventArgs e)
        {
            Enabled = false;
            await Push_Grant();
            if (Program.ClientContext.ParentForm != null && Program.ClientContext.ParentForm.UserManagementForm != null)
                Program.ClientContext.ParentForm.UserManagementForm.toolStripButtonRefresh_Click(null, null);
            Enabled = true;
        }

        private async void buttonRevokePush_Click(object sender, EventArgs e)
        {
            Enabled = false;
            await Push_Revoke();
            if (Program.ClientContext.ParentForm != null && Program.ClientContext.ParentForm.UserManagementForm != null)
                Program.ClientContext.ParentForm.UserManagementForm.toolStripButtonRefresh_Click(null, null);
            Enabled = true;
        }
    }
}
