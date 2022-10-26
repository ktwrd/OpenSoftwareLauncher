using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
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
        private bool localeApplied = false;
        public void Locale()
        {
            if (localeApplied) return;
            localeApplied = true;
            labelServer.Text = LocaleManager.Get("Server");
            labelUsername.Text = LocaleManager.Get("Username");
            labelPassword.Text = LocaleManager.Get("Password");
            checkBoxRemember.Text = LocaleManager.Get("Remember");

            buttonLogin.Text = LocaleManager.Get("Login");
            buttonOptions.Text = LocaleManager.Get("Option_Plural");

            Text = LocaleManager.Get("Title_LoginForm");
        }
        public LoginForm(bool validate = false, bool silent = false) : this()
        {
            if (UserConfig.Auth_Remember == false)
                validate = false;
            ValidateOnShow = validate || UserConfig.Auth_Remember;
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

            if (textBoxUsername.Text.Length < 1)
            {
                MessageBox.Show("Username is required", "Error");
                return;
            }
            if (textBoxPassword.Text.Length < 1)
            {
                MessageBox.Show("Password is required", "Error");
                return;
            }
            var serverUrlRegex = new Regex(@"^http(s|)://([a-zA-Z_\-0-9\+]\.){0,}[a-zA-Z]{1,63}$", RegexOptions.IgnoreCase);
            if (textBoxServer.Text.Length < 1)
            {
                MessageBox.Show("Server is required", "Error");
                return;
            }
            else
            {
                var match = serverUrlRegex.Match(textBoxServer.Text);
                if (!match.Success)
                {
                    MessageBox.Show("Invalid server URL", "Error");
                    return;
                }
            }

            Enabled = false;
            SaveFields();
            var response = Program.Client.ValidateCredentials(textBoxUsername.Text, textBoxPassword.Text, textBoxServer.Text);
            if (response.Success)
            {
                if (checkBoxRemember.Checked)
                {
                    UserConfig.Auth_Token = response.Token.Token;
                    UserConfig.Save();
                }

                Program.Client.UpdateProperties();
                Program.ClientContext.InitializeParentForm(true);
            }
            else
            {
                string errContent = JsonSerializer.Serialize(response, new JsonSerializerOptions()
                {
                    IncludeFields = true,
                    IgnoreReadOnlyFields = true,
                    IgnoreReadOnlyProperties = true,
                    WriteIndented = true
                });
                Trace.WriteLine($"[LoginForm->buttonLogin_Click] Rcieved failure response from server\n================\n{errContent}\n================\n");
                Program.MessageBoxShow(
                    LocaleManager.Get("Client_TokenGrantFailed") + "\n\n" + LocaleManager.Get(response.Message),
                    LocaleManager.Get("Client_TokenGrantFailed_Title"));
            }
            Enabled = true;
        }

        private void LoginForm_Shown(object sender, EventArgs e)
        {
            Locale();

            textBoxUsername.Text = UserConfig.Auth_Username;
            checkBoxRemember.Checked = UserConfig.Auth_Remember;
            textBoxServer.Text = UserConfig.Connection_Endpoint;

            textBoxUsername.Focus();
            if (textBoxUsername.Text.Length > 3 && textBoxPassword.Text.Length < 1)
            {
                textBoxPassword.Focus();
            }
            else if (textBoxUsername.Text.Length < 3 && textBoxPassword.Text.Length < 1)
            {
                textBoxUsername.Focus();
            }

            if (ValidateOnShow && UserConfig.Auth_Token.Length > 8)
            {
                Enabled = false;
                var response = Program.Client.ValidateToken(save: true);
                if (response != null)
                {
                    Program.Client.FetchAccountDetails();
                    Program.Client.FetchServerDetails();
                    Program.LocalContent.Pull().Wait();
                    Program.Client.UpdateProperties();
                    Program.ClientContext.InitializeParentForm(true);
                    return;
                }
                Enabled = true;
            }
        }

        private void textBoxPassword_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buttonLogin.PerformClick();
            }
        }
    }
}
