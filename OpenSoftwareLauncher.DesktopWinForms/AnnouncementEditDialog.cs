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
    public partial class AnnouncementEditDialog : Form
    {
        public AnnouncementEditDialog()
        {
            InitializeComponent();
        }
        public void Locale()
        {
            Text = LocaleManager.Get("Title_AnnouncementEditDialog");
            checkBoxEnable.Text = LocaleManager.Get("EnableAnnouncement");
            buttonPush.Text = LocaleManager.Get("Push");
        }

        private void AnnouncementEditDialog_Shown(object sender, EventArgs e)
        {
            Locale();
        }
    }
}
