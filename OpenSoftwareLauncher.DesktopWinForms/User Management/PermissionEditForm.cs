using kate.shared.Helpers;
using OSLCommon.Authorization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using System.Windows.Forms;

namespace OpenSoftwareLauncher.DesktopWinForms
{
    public partial class PermissionEditForm : Form
    {
        public PermissionEditForm()
        {
            InitializeComponent();
            Locale();
        }
        public void Locale()
        {
            Text = LocaleManager.Get("Permission_Plural") + $" - {Account.Username}";
            labelUsername.Text = LocaleManager.Get("EditingPermissionFor", inject: new Dictionary<string, object>()
            {
                {"username", Account.Username }
            });

            buttonPush.Text = LocaleManager.Get("Push");
        }


        public AccountDetailsResponse Account;
        public PermissionEditForm(AccountDetailsResponse account)
        {
            this.Account = account;
            InitializeComponent();
            Locale();
            RefreshCheckbox();
        }

        public void RefreshCheckbox()
        {
            checkedListBoxPermission.Items.Clear();

            var items = GeneralHelper.GetEnumList<AccountPermission>();
            foreach (var i in items)
            {
                checkedListBoxPermission.Items.Add(i.ToString());
            }

            for (int i = 0; i < this.Account.Permissions.Length; i++)
            {
                checkedListBoxPermission.SetItemChecked(checkedListBoxPermission.Items.IndexOf((object)this.Account.Permissions[i].ToString()), true);
            }
        }

        private void buttonPush_Click(object sender, EventArgs e)
        {
            Enabled = false;
            var toGrant = new List<AccountPermission>();
            var toRevoke = new List<AccountPermission>();

            for (int i = 0; i < checkedListBoxPermission.Items.Count; i++)
            {
                string item = (string)checkedListBoxPermission.Items[i];
                bool isChecked = checkedListBoxPermission.GetItemChecked(i);


                AccountPermission perm;
                bool success = Enum.TryParse<AccountPermission>(item, out perm);
                if (!success) continue;

                if (Account.Permissions.Contains(perm) && !isChecked)
                {
                    toRevoke.Add(perm);
                }
                else if (!Account.Permissions.Contains(perm) && isChecked)
                {
                    toGrant.Add(perm);
                }
            }

            var tasks = new List<Task>();
            foreach (var i in toGrant)
            {
                tasks.Add(new Task(delegate
                {
                    var res = Program.Client.PermissionGrant(Account.Username, i).Result;
                    if (res != null)
                        MessageBox.Show(LocaleManager.Get(res.Message, @"Failed to grant permission"));
                }));
            }
            foreach (var i in toRevoke)
            {
                tasks.Add(new Task(delegate
                {
                    var res = Program.Client.PermissionRevoke(Account.Username, i).Result;
                    if (res != null)
                        MessageBox.Show(LocaleManager.Get(res.Message, @"Failed to revoke permission"));
                }));
            }

            foreach (var i in tasks)
                i.Start();
            Task.WhenAll(tasks).Wait();
            Enabled = true;
            if (Program.ClientContext.ParentForm != null && Program.ClientContext.ParentForm.UserManagementForm != null)
            {
                Program.ClientContext.ParentForm.UserManagementForm.Pull();
            }
            Close();
        }
    }
}
