using OpenSoftwareLauncher.DesktopWinForms.ServerBridge;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace OpenSoftwareLauncher.DesktopWinForms
{
    public class ClientContext : ApplicationContext
    {
        public LoginForm LoginForm;
        public ParentForm ParentForm;

        public ClientContext()
        {
            try
            {
                Application.ApplicationExit += Application_ApplicationExit;

                Program.Client = new Client();
                Program.LocalContent = new LocalContent();

                PromptLoginWindow();
            }
            catch (Exception e)
            {
                Program.MessageBoxShow(LocaleManager.Get("ClientContext_ConstructorFail", inject: new Dictionary<string, object>()
                {
                    { @"exception", e.ToString() }
                }));
                Trace.WriteLine($"[ClientContext->constructor] Uncaught exception when initializing ClientContext\n================\n{e}\n================\n");
            }
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
        }
        public void Restart()
        {
            var loc = Assembly.GetExecutingAssembly().Location;
            if (loc.EndsWith(".exe"))
                Process.Start(loc);
            Application.ExitThread();
            Application.Exit();
        }

        public void PromptLoginWindow(bool validate = false, bool silent = false)
        {
            try
            {
                if (LoginForm == null || LoginForm.IsDisposed)
                    LoginForm = new LoginForm(validate, silent);
                MainForm = LoginForm;
                LoginForm.Show();
            }
            catch (Exception e)
            {
                Program.MessageBoxShow(LocaleManager.Get("ClientContext_LoginFormFail", inject: new Dictionary<string, object>()
                {
                    { @"exception", e.ToString() }
                }));
                Trace.WriteLine($"[ClientContext->PromptLoginWindow] Uncaught exception when showing LoginForm\n================\n{e}\n================\n");
            }
        }
        public void InitializeParentForm(bool closeLoginWindow=true)
        {
            try
            {
                if (ParentForm == null || ParentForm.IsDisposed)
                    ParentForm = new ParentForm();
                MainForm = ParentForm;
                if (closeLoginWindow && (LoginForm != null && !LoginForm.IsDisposed))
                    LoginForm.Close();
                ParentForm.Show();
            }
            catch (Exception e)
            {
                Program.MessageBoxShow(LocaleManager.Get("ClientContext_ParentFormFail", inject: new Dictionary<string, object>()
                {
                    { @"exception", e.ToString() }
                }));
                Trace.WriteLine($"[ClientContext->InitializeParentForm] Uncaught exception when showing ParentForm\n================\n{e}\n================\n");
            }
        }
    }
}
