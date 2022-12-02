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
    public partial class BulkUserLicenseForm : Form
    {
        public BulkUserLicenseForm()
        {
            InitializeComponent();
        }
        public void Locale()
        {
            Text = LocaleManager.Get(Text);

            LocaleControl(this);
        }
        public void LocaleControl(Control control)
        {
            if (control.Controls.Count > 0)
                foreach (Control elem in control.Controls)
                    LocaleControl(elem);
            foreach (Label elem in control.Controls.OfType<Label>())
                if (elem.Text.Length > 0)
                    elem.Text = LocaleManager.Get(elem.Text);
            foreach (Button elem in control.Controls.OfType<Button>())
                if (elem.Text.Length > 0)
                    elem.Text = LocaleManager.Get(elem.Text);
            foreach (GroupBox elem in control.Controls.OfType<GroupBox>())
                if (elem.Text.Length > 0)
                    elem.Text = LocaleManager.Get(elem.Text);
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
            => ReloadGenericListUsernames(checkedListBoxRevoke_Username);
        public void ReloadRevokeListLicenses()
            => RelaodGenericListLicenses(checkedListBoxRevoke_Licenses);
        #endregion

        #region Grant List Reload
        public void ReloadGrantList()
        {
            ReloadGrantListUsernames();
            ReloadGrantListLicenses();
        }
        public void ReloadGrantListUsernames()
        {
            ReloadGenericListUsernames(checkedListBoxGrant_Username);
        }
        public void ReloadGrantListLicenses()
        {
            RelaodGenericListLicenses(checkedListBoxGrant_Licenses);
        }
        #endregion

        #region Helper Methods
        public void ReloadGenericCheckedboxList(CheckedListBox cbox, object[] newitems)
        {
            var CheckedItems = new List<object>();
            foreach (var item in cbox.CheckedItems)
                CheckedItems.Add(item);

            cbox.Items.Clear();
            int index = 0;
            foreach (var item in newitems)
            {
                cbox.Items.Add(item);
                if (CheckedItems.Contains(item))
                    cbox.SetItemChecked(index, true);
                index++;
            }
        }
        public void ReloadGenericListUsernames(CheckedListBox cbox)
        {
            var newitems = Program.LocalContent.AccountDetailList
                .Select(v => v.Username)
                .Concat(cbox.Items.Cast<string>())
                .Distinct()
                .ToArray();
            ReloadGenericCheckedboxList(
                cbox,
                newitems);
        }
        public void RelaodGenericListLicenses(CheckedListBox cbox)
        {
            var newitems = cbox.Items.Cast<string>()
                .Concat(Program.LocalContent.GetRemoteLocations())
                .Distinct()
                .ToArray();
            ReloadGenericCheckedboxList(cbox, newitems);
        }
        public void SelectAll(CheckedListBox cbox)
        {
            for (int i = 0; i < cbox.Items.Count; i++)
            {
                cbox.SetItemChecked(i, true);
            }
        }
        public void SelectInverse(CheckedListBox cbox)
        {
            for (int i = 0; i < cbox.Items.Count; i++)
            {
                cbox.SetItemChecked(i, !cbox.GetItemChecked(i));
            }
        }
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
                    foreach (var item in account.Licenses)
                    {
                        if (!selectedLicenses.Contains(item) && allLicenses.Contains(item))
                            licenseList.Add(item);
                    }
                    foreach (var item in selectedLicenses)
                        if (!licenseList.Contains(item) && !account.Licenses.Contains(item))
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
            var allLicenses = licenseBox.Items.Cast<string>();
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
            => SelectAll(checkedListBoxGrant_Username);

        private void buttonGrant_SelectInvert_Click(object sender, EventArgs e)
            => SelectInverse(checkedListBoxGrant_Username);

        private void buttonRevoke_SelectAll_Click(object sender, EventArgs e)
            => SelectAll(checkedListBoxRevoke_Username);

        private void buttonRevoke_SelectInvert_Click(object sender, EventArgs e)
            => SelectInverse(checkedListBoxRevoke_Username);



        private void buttonGrant_License_SelectAll_Click(object sender, EventArgs e)
            => SelectAll(checkedListBoxGrant_Licenses);
        private void buttonGrant_License_SelectInvert_Click(object sender, EventArgs e)
            => SelectInverse(checkedListBoxGrant_Licenses);

        private void buttonRevoke_License_SelectAll_Click(object sender, EventArgs e)
            => SelectAll(checkedListBoxRevoke_Licenses);
        private void buttonRevoke_License_SelectInvert_Click(object sender, EventArgs e)
            => SelectInverse(checkedListBoxRevoke_Licenses);
        #endregion

        private async void buttonGrant_Push_Click(object sender, EventArgs e)
        {
            Enabled = false;
            await Push_Grant();
            Enabled = true;
        }

        private async void buttonRevoke_Push_Click(object sender, EventArgs e)
        {
            Enabled = false;
            await Push_Revoke();
            Enabled = true;
        }

        private void BulkUserLicenseForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Program.ClientContext.ParentForm != null && Program.ClientContext.ParentForm.UserManagementForm != null)
                Program.ClientContext.ParentForm.UserManagementForm.toolStripButtonRefresh_Click(null, null);
        }
    }
}
