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
        public void SaveFields()
        {
            UserConfig.Auth_Username = textBoxUsername.Text;
            UserConfig.Auth_Remember = checkBoxRemember.Checked;
            UserConfig.Connection_Endpoint = textBoxServer.Text;
            UserConfig.Save();
        }
        private void buttonLogin_Click(object sender, EventArgs e)
        {
            Enabled = false;
            SaveFields();
            var response = Program.Client.ValidateCredentials(textBoxUsername.Text, textBoxPassword.Text, textBoxServer.Text);
            if (response.Success)
            {
                Program.Client.UpdateProperties();
                Program.ClientContext.InitializeParentForm(true);
            }
        }

        private void LoginForm_Shown(object sender, EventArgs e)
        {
            textBoxUsername.Text = UserConfig.Auth_Username;
            checkBoxRemember.Checked = UserConfig.Auth_Remember;
            textBoxServer.Text = UserConfig.Connection_Endpoint;

            if (textBoxUsername.Text.Length > 0)
            {
                textBoxPassword.Focus();
            }
            else
            {
                textBoxUsername.Focus();
            }
        }
    }
}
