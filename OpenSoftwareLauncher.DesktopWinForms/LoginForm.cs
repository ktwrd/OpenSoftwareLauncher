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
            Locale();
        }
        public void Locale()
        {
            labelServer.Text = LocaleManager.Get("Server");
            labelUsername.Text = LocaleManager.Get("FieldUsername");
            labelPassword.Text = LocaleManager.Get("FieldPassword");
            checkBoxRemember.Text = LocaleManager.Get("FieldRemember");

            buttonLogin.Text = LocaleManager.Get("Login");
            buttonOptions.Text = LocaleManager.Get("Options");

            Text = LocaleManager.Get("WindowTitle_LoginForm");
        }
        public LoginForm(bool validate = false, bool silent = false)
        {
            InitializeComponent();
            Locale();
            if (UserConfig.Auth_Remember == false)
                validate = false;
            ValidateOnShow = validate;
        }
        private bool ValidateOnShow = false;
    }
}
