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
    public partial class UserDisableForm : Form
    {
        public UserDisableForm()
        {
            InitializeComponent();
        }

        public void Locale()
        {
            Text = LocaleManager.Get("Banish") + $" - {Account.Username}";
            buttonSubmit.Text = LocaleManager.Get("Submit");
        }

        public AccountDetailsResponse Account;
        public UserDisableForm(AccountDetailsResponse account)
        {
            Account = account;
            InitializeComponent();
            Locale();
        }

        private void buttonSubmit_Click(object sender, EventArgs e)
        {
            Enabled = false;
            var response = Program.Client.AccountBan(Account.Username, textBoxReason.Text).Result;
            if (response != null)
            {
                MessageBox.Show(LocaleManager.Get(response.Message, @"Failed to ban user"));
            }
            if (Program.ClientContext.ParentForm != null && Program.ClientContext.ParentForm.UserManagementForm != null)
            {
                Program.ClientContext.ParentForm.UserManagementForm.ReloadList(false);
            }
            Close();
        }
    }
}
