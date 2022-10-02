using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenSoftwareLauncher.DesktopWinForms
{
    public class ClientContext : ApplicationContext
    {
        public LoginForm LoginForm;

        public ClientContext()
        {
            Application.ApplicationExit += Application_ApplicationExit;
        }

        private void Application_ApplicationExit(object? sender, EventArgs e)
        {
        }
        public void Restart()
        {
            Application.Restart();
        }

        public void PromptLoginWindow(bool validate = false, bool silent = false)
        {
            if (LoginForm == null || LoginForm.IsDisposed)
                LoginForm = new LoginForm(validate, silent);
            MainForm = LoginForm;
            LoginForm.Show();
        }
        public void InitializeClientForm()
        {

        }
    }
}
