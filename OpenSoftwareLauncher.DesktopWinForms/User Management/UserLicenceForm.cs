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
    public partial class UserLicenceForm : Form
    {
        public UserLicenceForm()
        {
            InitializeComponent();
        }
        public AccountDetailsResponse Account;
        public UserLicenceForm(AccountDetailsResponse account)
        {
            InitializeComponent();
            Account = account;
            TargetLicenses = new List<string>(Account.Licenses);
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
                if (!listBoxLicenses.Items.Contains((object)i) && i.Length > 1)
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
            ReloadComboBox();
            ReloadLicenseList();
        }
        public void RemoveLicense(string license)
        {
            TargetLicenses.Remove(license);
            ReloadComboBox();
            ReloadLicenseList();
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

        private void UserLicenceForm_Shown(object sender, EventArgs e)
        {
            ReloadComboBox();
            ReloadLicenseList();
        }

        private void buttonPush_Click(object sender, EventArgs e)
        {
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
        }
    }
}
