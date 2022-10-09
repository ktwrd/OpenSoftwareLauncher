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
            if (!Program.Client.Permissions.Contains(AccountPermission.USER_LIST))
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
                        string.Join(", ", item.Groups),
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
    }
}
