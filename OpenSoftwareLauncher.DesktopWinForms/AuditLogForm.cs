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
    public partial class AuditLogForm : Form
    {
        public AuditLogForm()
        {
            InitializeComponent();
        }

        public void Locale()
        {
            toolStripButtonRefresh.Text = LocaleManager.Get("Refresh");
            toolStripDropDownButtonFilter.Text = LocaleManager.Get("Filter");
            foreach (ColumnHeader item in listView1.Columns)
            {
                item.Text = LocaleManager.Get(item.Text);
            }
            Text = LocaleManager.Get("Title_AuditLog");
        }

        private void AuditLogForm_Shown(object sender, EventArgs e)
        {
            Locale();
        }
    }
}
