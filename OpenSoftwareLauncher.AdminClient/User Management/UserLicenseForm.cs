using OSLCommon.Authorization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenSoftwareLauncher.AdminClient
{
    public partial class UserLicenseForm : Form
    {
        public UserLicenseForm()
        {
            InitializeComponent();
        }
        public AccountDetailsResponse Account;
        public UserLicenseForm(AccountDetailsResponse account)
        {
            InitializeComponent();
            Account = account;
            labelEmail.Text = Account.Username;
            Locale();
            TargetLicenses = new List<string>(Account.Licenses);
        }

        public void Locale()
        {
            Text = LocaleManager.Get("License_Plural") + $" - {Account.Username}";
            buttonPush.Text = LocaleManager.Get("Push");
            buttonRemoveLicense.Text = LocaleManager.Get("Remove");
        }

        public List<string> TargetLicenses = new List<string>();

        public void ReloadLicenseList()
        {
            listBoxLicenses.Items.Clear();
            foreach (var i in TargetLicenses)
                listBoxLicenses.Items.Add(i);
            buttonGrant.Enabled = comboBoxCurrentLicense.SelectedItem == null || comboBoxCurrentLicense.SelectedItem.ToString().Length > 1;
            buttonRemoveLicense.Enabled = listBoxLicenses.SelectedIndex >= 0;
        }

        public void ReloadComboBox()
        {
            comboBoxCurrentLicense.Items.Clear();
            comboBoxCurrentLicense.Items.Add("");

            foreach (var i in Program.LocalContent.GetRemoteLocations())
                if (!listBoxLicenses.Items.Contains((object)i) && i.Length > 4)
                    comboBoxCurrentLicense.Items.Add(i);
            comboBoxCurrentLicense.SelectedIndex = 0;
            buttonGrant.Enabled = comboBoxCurrentLicense.SelectedItem == null || comboBoxCurrentLicense.SelectedItem?.ToString().Length > 1;
            buttonRemoveLicense.Enabled = listBoxLicenses.SelectedIndex >= 0;
        }

        public void AddLicense(string license)
        {
            if (TargetLicenses.Contains(license))
                return;
            TargetLicenses.Add(license);
            ReloadLicenseList();
            ReloadComboBox();
        }
        public void RemoveLicense(string license)
        {
            TargetLicenses.Remove(license);
            ReloadLicenseList();
            ReloadComboBox();
        }

        private void comboBoxCurrentLicense_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonGrant.Enabled = comboBoxCurrentLicense.SelectedItem == null || comboBoxCurrentLicense.SelectedItem.ToString().Length > 1;
        }

        private void buttonGrant_Click(object sender, EventArgs e)
        {
            AddLicense(comboBoxCurrentLicense.SelectedItem.ToString());
        }

        private void listBoxLicenses_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonRemoveLicense.Enabled = listBoxLicenses.SelectedIndex >= 0;
        }

        private void UserLicenseForm_Shown(object sender, EventArgs e)
        {
            ReloadLicenseList();
            ReloadComboBox();
        }

        private void buttonPush_Click(object sender, EventArgs e)
        {
            Enabled = false;
            var remove = new List<string>();
            var grant = new List<string>();

            TargetLicenses = new List<string>();
            foreach (var i in listBoxLicenses.Items)
                TargetLicenses.Add(i.ToString());

            foreach (var license in TargetLicenses)
                if (!Account.Licenses.Contains(license))
                    grant.Add(license);

            foreach (var license in Account.Licenses)
                if (!TargetLicenses.Contains(license))
                    remove.Add(license);


            var taskList = new List<Task>();
            foreach (var license in remove)
                taskList.Add(new Task(async delegate
                {
                    var url = Endpoint.UserLicenseRevoke(
                        Program.Client.Token,
                        Account.Username,
                        license);
                    await Program.Client.HttpClient.GetAsync(url);
                }));
            foreach (var license in grant)
                taskList.Add(new Task(async delegate
                {
                    var url = Endpoint.UserLicenseGrant(
                        Program.Client.Token,
                        Account.Username,
                        license);
                    await Program.Client.HttpClient.GetAsync(url);
                }));

            foreach (var i in taskList)
                i.Start();
            Task.WhenAll(taskList);
            Program.LocalContent.PullAccounts();
            Close();
        }

        private void buttonRemoveLicense_Click(object sender, EventArgs e)
        {
            listBoxLicenses.Items.Remove(listBoxLicenses.SelectedItem);
        }
    }
}
