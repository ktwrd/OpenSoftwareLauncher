using CommandLine;
using kate.shared;
using OpenSoftwareLauncher.DesktopWinForms.ServerBridge;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenSoftwareLauncher.DesktopWinForms
{
    public class CommandlineOptions
    {
        [Option('t', "token", Required = false, Default = "")]
        public string Token { get; set; }
        [Option('u', "username", Required = false, Default = "")]
        public string Username { get; set; }
        [Option('e', "endpoint", Required = false, Default = "")]
        public string Endpoint { get; set; }
        [Option('l', "autologin", Required = false, Default = false)]
        public bool AutoLogin { get; set; }
    }

    public static class Program
    {
        public static JsonSerializerOptions serializerOptions = new JsonSerializerOptions()
        {
            IgnoreReadOnlyFields = true,
            IgnoreReadOnlyProperties = true,
            IncludeFields = true,
            WriteIndented = true
        };

        public static Client Client;
        public static LocalContent LocalContent;
        public static ClientContext ClientContext;

        public static string SoftwareVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static string ProductName = @"Open Software Launcher";
        public static string ProductNameAcronym = "OSL";
        public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DebugListener DebugListener;
        public static System.Windows.Forms.Timer LogFlushTimer;

        public static CommandlineOptions Options;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
#if DEBUG
            System.Threading.Thread.Sleep(2000);
#endif

            UserConfig.Get();
            LocaleManager.Load();
            try
            {
                var parser = new Parser(with => with.IgnoreUnknownArguments = true);
                var parsed = parser.ParseArguments<CommandlineOptions>(args);
                Options = parsed.Value;
                if (Options.Token.Length > 0)
                    UserConfig.Auth_Token = Options.Token;
                if (Options.Username.Length > 0)
                    UserConfig.Auth_Username = Options.Username;
                if (Options.Endpoint.Length > 0)
                    UserConfig.Connection_Endpoint = Options.Endpoint;
                if (Options.AutoLogin)
                    UserConfig.Auth_Remember = Options.AutoLogin;


                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                DebugListener = new DebugListener();
                LogFlushTimer = new System.Windows.Forms.Timer();
                LogFlushTimer.Interval = 500;
                LogFlushTimer.Tick += LogFlushTimer_Tick;
                DebugListener.Updated += DebugListener_Updated;
                LogFlushTimer.Start();
                ClientContext = new ClientContext();
                Application.Run(ClientContext);
            }
            
            catch (Exception e)
            {
                Program.MessageBoxShow(LocaleManager.Get("FatalFailure_Exception", inject: new Dictionary<string, object>()
                {
                    { @"exception", e.ToString() }
                }));
                Trace.WriteLine($"[Program->Main] Fatal Exception\n================\n{e}\n================\n");
            }
        }

        private static bool PendingLogLines = false;
        private static void DebugListener_Updated(DebugListener instance)
        {
            if (!PendingLogLines)
                PendingLogLines = true;
        }

        private static void LogFlushTimer_Tick(object sender, EventArgs e)
        {
            if (PendingLogLines)
            {
                PendingLogLines = false;
                var content = new List<string>(DebugListener.LogLines).ToArray();
                if (ClientContext != null && ClientContext.ParentForm != null && ClientContext.ParentForm.LogForm != null)
                    ClientContext.ParentForm.LogForm.SetContent(content);
            }
            LogFlushTimer.Start();
        }

        public static DialogResult MessageBoxShow(
            string text="",
            string caption=null,
            MessageBoxButtons buttons= MessageBoxButtons.OK,
            MessageBoxIcon icon= MessageBoxIcon.None,
            MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1,
            MessageBoxOptions options = MessageBoxOptions.DefaultDesktopOnly,
            bool displayHelpButton=false)
        {
            if (caption == null)
            {
                caption = LocaleManager.Get("Title");
                Trace.WriteLine("[MessageBoxShow] \"caption\" Parameter is null, defaulting to locale \"MessageBox_Title\"");
            }
            return MessageBox.Show(text, caption, buttons, icon, defaultButton, options, displayHelpButton);
        }
    }
}
