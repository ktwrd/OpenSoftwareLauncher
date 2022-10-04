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
    }
}
