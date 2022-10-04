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
    public partial class ParentForm : Form
    {
        public UserDetailsForm userDetailsForm;
        public ParentForm()
        {
            InitializeComponent();
            Locale();
        }

        public void Locale()
        {
            toolStripButtonUsers.Text = LocaleManager.Get(toolStripButtonUsers.Text);
            toolStripButtonAnnouncements.Text = LocaleManager.Get(toolStripButtonAnnouncements.Text);
            toolStripButtonReleases.Text = LocaleManager.Get(toolStripButtonReleases.Text);
            toolStripButtonSettings.Text = LocaleManager.Get(toolStripButtonSettings.Text);
            toolStripButtonLogs.Text = LocaleManager.Get(toolStripButtonLogs.Text);
            Text = LocaleManager.Get(Text);
        }

        private void ParentForm_Shown(object sender, EventArgs e)
        {
            userDetailsForm = new UserDetailsForm();
            userDetailsForm.MdiParent = this;
            userDetailsForm.Location = new Point(32, 0);
            userDetailsForm.Show();
        }
    }
}
