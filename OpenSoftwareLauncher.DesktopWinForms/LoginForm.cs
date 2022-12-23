using OSLCommon;
using OSLCommon.Authorization;
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
            switch (GetSelectedTabMethod())
            {
                case LoginMethod.Username:
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
                    break;
                case LoginMethod.Token:
                    if (textBoxToken.Text.Length < 14)
                    {
                        MessageBox.Show("Token is required", "Error");
                        return;
                    }
                    break;
            }
            if (textBoxServer.Text.Length < 1)
            {
                MessageBox.Show("Server is required", "Error");
                return;
            }

            var serverUrlRegex = new Regex(@"^http(s|)://([a-zA-Z_\-0-9\+]+\.){0,}[a-zA-Z]{1,63}((:[0-9]{1,6})|)$", RegexOptions.IgnoreCase);
            var match = serverUrlRegex.Match(textBoxServer.Text);
            if (!match.Success)
            {
                MessageBox.Show("Invalid server URL", "Error");
                return;
            }

            Enabled = false;
            SaveFields();
            object response = new object();
            switch (GetSelectedTabMethod())
            {
                case LoginMethod.Username:
                    GrantTokenResponse usernameValidateResponse = Program.Client.Auth.ValidateCredentials(textBoxUsername.Text, textBoxPassword.Text, textBoxServer.Text, showMessageBox: true);
                    response = usernameValidateResponse;
                    if (usernameValidateResponse == null || !usernameValidateResponse.Success)
                        break;

                    UserConfig.Auth_Token = usernameValidateResponse.Token.Token;
                    UserConfig.Save();
                    break;
                case LoginMethod.Token:
                    AccountTokenDetailsResponse tokenValidateResponse = Program.Client.Auth.ValidateToken(textBoxToken.Text, textBoxServer.Text, showMessageBox: true);
                    response = tokenValidateResponse;
                    if (tokenValidateResponse == null)
                        break;

                    // Save token if valid, and allow us to continue
                    UserConfig.Auth_Token = textBoxToken.Text;
                    UserConfig.Save();
                    break;
            }
            if (response != null)
            {
                Trace.WriteLine($"[LoginForm.buttonLogin_Click] Validation Success!");
                UserConfig.Connection_Endpoint = textBoxServer.Text;
                UserConfig.Save();
                Program.Client.UpdateProperties();
                Program.ClientContext.InitializeParentForm(true);
                Enabled = true;
                return;
            }

            string errContent = JsonSerializer.Serialize(response, new JsonSerializerOptions()
            {
                IncludeFields = true,
                IgnoreReadOnlyFields = true,
                IgnoreReadOnlyProperties = true,
                WriteIndented = true
            });
            Trace.WriteLine($"[LoginForm->buttonLogin_Click] Rcieved failure response from server\n================\n{errContent}\n================\n");
            /*Program.MessageBoxShow(
                LocaleManager.Get("Client_TokenGrantFailed") + "\n\n" + LocaleManager.Get(errorResponse.Message),
                LocaleManager.Get("Client_TokenGrantFailed_Title"));*/
            Enabled = true;
        }

        private void LoginForm_Shown(object sender, EventArgs e)
        {
            Locale();

            textBoxUsername.Text = UserConfig.Auth_Username;
            checkBoxRemember.Checked = UserConfig.Auth_Remember;
            textBoxServer.Text = UserConfig.Connection_Endpoint;

            switch (GetSelectedTabMethod())
            {
                case LoginMethod.Username:
                    textBoxUsername.Focus();
                    if (textBoxUsername.Text.Length > 3 && textBoxPassword.Text.Length < 1)
                        textBoxPassword.Focus();
                    else if (textBoxUsername.Text.Length < 3 && textBoxPassword.Text.Length < 1)
                        textBoxUsername.Focus();
                    break;
                case LoginMethod.Token:
                    textBoxToken.Focus();
                    break;
            }
            if (!ValidateOnShow || UserConfig.Auth_Token.Length < 8)
                return;

            Enabled = false;
            var response = Program.Client.Auth.ValidateToken(save: true);
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

        private void SelectTab(LoginMethod method)
        {
            tabControl1.SelectTab((int)method);
        }
        private LoginMethod GetSelectedTabMethod()
        {
            if (tabControl1.SelectedTab == tabPage1)
                return LoginMethod.Username;
            else if (tabControl1.SelectedTab == tabPage2)
                return LoginMethod.Token;
            return LoginMethod.Unknown;
        }

        private void textBoxPassword_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buttonLogin.PerformClick();
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UserConfig.Set("Authentication", "LoginType", GetSelectedTabMethod());
        }
    }
}
