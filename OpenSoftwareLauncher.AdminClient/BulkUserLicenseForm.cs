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

namespace OpenSoftwareLauncher.AdminClient
{
    public partial class BulkUserLicenseForm : Form
    {
        public BulkUserLicenseForm()
        {
            InitializeComponent();
        }
        public void Locale()
        {
            Text = LocaleManager.Get(Text);

            FormHelper.LocaleControl(this);
        }
        private void BulkUserLicenseForm_Shown(object sender, EventArgs e)
        {
            Locale();
            Pull();
        }
        private async Task Pull()
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

        public async Task Push_Grant(bool pullAccounts=true)
        {
            var accountDictionary = GenerateAccountLicensePairs(checkedListBoxGrant_Username, checkedListBoxGrant_Licenses);

            var taskList = new List<Task>();
            foreach (var pair in accountDictionary)
            {
                foreach (var license in pair.Value)
                {
                    taskList.Add(new Task(delegate
                    {
                        var res = Program.Client.HttpClient.GetAsync(Endpoint.UserLicenseGrant(Program.Client.Token, pair.Key, license)).Result;
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
        public async Task Push_Revoke(bool pullAccounts=true)
        {
            var accountDictionary = GenerateAccountLicensePairsRevoke(checkedListBoxRevoke_Username, checkedListBoxRevoke_Licenses);

            var taskList = new List<Task>();
            foreach (var pair in accountDictionary)
            {
                foreach (var license in pair.Value)
                {
                    taskList.Add(new Task(delegate
                    {
                        var res = Program.Client.HttpClient.GetAsync(Endpoint.UserLicenseRevoke(Program.Client.Token, pair.Key, license)).Result;
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


        #region Revoke List Reload
        public void ReloadRevokeList()
        {
            ReloadRevokeListUsernames();
            ReloadRevokeListLicenses();
        }
        public void ReloadRevokeListUsernames()
            => FormHelper.ReloadGenericListUsernames(checkedListBoxRevoke_Username);
        public void ReloadRevokeListLicenses()
            => FormHelper.RelaodGenericListLicenses(checkedListBoxRevoke_Licenses);
        #endregion

        #region Grant List Reload
        public void ReloadGrantList()
        {
            ReloadGrantListUsernames();
            ReloadGrantListLicenses();
        }
        public void ReloadGrantListUsernames()
        {
            FormHelper.ReloadGenericListUsernames(checkedListBoxGrant_Username);
        }
        public void ReloadGrantListLicenses()
        {
            FormHelper.RelaodGenericListLicenses(checkedListBoxGrant_Licenses);
        }
        #endregion

        #region Helper Methods
        private Dictionary<string, string[]> GenerateAccountLicensePairs(CheckedListBox userList, CheckedListBox licenseBox)
        {
            var accountDictionary = new Dictionary<string, string[]>();

            var selectedAccounts = userList.CheckedItems.Cast<string>();
            var selectedLicenses = licenseBox.CheckedItems.Cast<string>();
            var allLicenses = licenseBox.Items.Cast<string>();
            foreach (var account in Program.LocalContent.AccountDetailList)
            {
                if (selectedAccounts.Contains(account.Username))
                {
                    var licenseList = new List<string>();
                    foreach (var item in selectedLicenses)
                        if (!account.Licenses.Contains(item))
                            licenseList.Add(item);
                    accountDictionary.Add(account.Username, licenseList.ToArray());
                }
            }

            return accountDictionary;
        }
        private Dictionary<string, string[]> GenerateAccountLicensePairsRevoke(CheckedListBox userList, CheckedListBox licenseBox)
        {
            var accountDictionary = new Dictionary<string, string[]>();
            var selectedAccounts = userList.CheckedItems.Cast<string>();
            var selectedLicenses = licenseBox.CheckedItems.Cast<string>();
            foreach (var account in Program.LocalContent.AccountDetailList)
            {
                if (selectedAccounts.Contains(account.Username))
                {
                    var licenseList = new List<string>();
                    foreach (var item in selectedLicenses)
                        if (account.Licenses.Contains(item))
                            licenseList.Add(item);
                    accountDictionary.Add(account.Username, licenseList.ToArray());
                }
            }
            return accountDictionary;
        }
        #endregion

        #region Custom License button
        private void buttonGrant_AddCustomLicense_Click(object sender, EventArgs e)
        {
            if (textBoxGrant_CustomLicense.Text.Length < 1) return;
            checkedListBoxGrant_Licenses.Items.Add(textBoxGrant_CustomLicense.Text);
            textBoxGrant_CustomLicense.Text = "";
        }

        private void buttonRevoke_AddCustomLicense_Click(object sender, EventArgs e)
        {
            if (textBoxRevoke_CustomLicense.Text.Length < 1) return;
            checkedListBoxRevoke_Licenses.Items.Add(textBoxRevoke_CustomLicense.Text);
            textBoxRevoke_CustomLicense.Text = "";
        }
        #endregion

        #region Selection buttons
        private void buttonGrant_SelectAll_Click(object sender, EventArgs e)
            => FormHelper.SelectAll(checkedListBoxGrant_Username);

        private void buttonGrant_SelectInvert_Click(object sender, EventArgs e)
            => FormHelper.SelectInverse(checkedListBoxGrant_Username);

        private void buttonRevoke_SelectAll_Click(object sender, EventArgs e)
            => FormHelper.SelectAll(checkedListBoxRevoke_Username);

        private void buttonRevoke_SelectInvert_Click(object sender, EventArgs e)
            => FormHelper.SelectInverse(checkedListBoxRevoke_Username);



        private void buttonGrant_License_SelectAll_Click(object sender, EventArgs e)
            => FormHelper.SelectAll(checkedListBoxGrant_Licenses);
        private void buttonGrant_License_SelectInvert_Click(object sender, EventArgs e)
            => FormHelper.SelectInverse(checkedListBoxGrant_Licenses);

        private void buttonRevoke_License_SelectAll_Click(object sender, EventArgs e)
            => FormHelper.SelectAll(checkedListBoxRevoke_Licenses);
        private void buttonRevoke_License_SelectInvert_Click(object sender, EventArgs e)
            => FormHelper.SelectInverse(checkedListBoxRevoke_Licenses);
        #endregion

        private async void buttonGrant_Push_Click(object sender, EventArgs e)
        {
            Enabled = false;
            await Push_Grant();
            if (Program.ClientContext.ParentForm != null && Program.ClientContext.ParentForm.UserManagementForm != null)
                Program.ClientContext.ParentForm.UserManagementForm.toolStripButtonRefresh_Click(null, null);
            Enabled = true;
        }

        private async void buttonRevoke_Push_Click(object sender, EventArgs e)
        {
            Enabled = false;
            await Push_Revoke();
            if (Program.ClientContext.ParentForm != null && Program.ClientContext.ParentForm.UserManagementForm != null)
                Program.ClientContext.ParentForm.UserManagementForm.toolStripButtonRefresh_Click(null, null);
            Enabled = true;
        }

        private void BulkUserLicenseForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Program.ClientContext.ParentForm != null && Program.ClientContext.ParentForm.UserManagementForm != null)
                Program.ClientContext.ParentForm.UserManagementForm.toolStripButtonRefresh_Click(null, null);
        }
    }
}
