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
    public partial class UserGroupForm : Form
    {
        public UserGroupForm()
        {
            InitializeComponent();
        }
        public AccountDetailsResponse Account;
        public UserGroupForm(AccountDetailsResponse account)
        {
            InitializeComponent();
            Account = account;
        }

        public void ReloadGroupList()
        {
            listViewGroups.Items.Clear();
            foreach (var i in Account.Groups)
            {
                this.listViewGroups.Items.Add(i);
            }
            textBoxTargetGroup.Text = "";
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            listViewGroups.Items.Add(textBoxTargetGroup.Text);
            textBoxTargetGroup.Text = "";
        }

        private void buttonRemoveSelected_Click(object sender, EventArgs e)
        {
            var items = new List<string>();

            for (int i = 0; i < listViewGroups.Items.Count; i++)
            {
                if (!listViewGroups.SelectedIndices.Contains(i))
                    items.Add(listViewGroups.Items[i].ToString());
            }

            listViewGroups.Items.Clear();
            foreach (var i in items)
                listViewGroups.Items.Add(i);
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            var fileLines = listViewGroups.Items.Cast<string>().ToArray();

            var dialog = new SaveFileDialog()
            {
                Title = LocaleManager.Get("FieldSave_GroupPlural"),
                Filter = "txt files (*.txt)|*.txtx|All files (*.*)|*.*",
                DefaultExt = "txt",
                RestoreDirectory = true
            };
            var result = dialog.ShowDialog();
            if (result != DialogResult.OK) return;

            var location = dialog.FileName;

            
        }
    }
}
