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
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }
        public LoginForm(bool validate = false, bool silent = false)
        {
            InitializeComponent();
            if (UserConfig.Auth_Remember == false)
                validate = false;
            ValidateOnShow = validate;
        }
        private bool ValidateOnShow = false;
    }
}
