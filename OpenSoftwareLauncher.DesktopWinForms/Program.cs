using OpenSoftwareLauncher.DesktopWinForms.ServerBridge;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenSoftwareLauncher.DesktopWinForms
{
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

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            UserConfig.Get();
            LocaleManager.Load();
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
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
                caption = LocaleManager.Get("MessageBox_Title");
                Trace.WriteLine("[MessageBoxShow] \"caption\" Parameter is null, defaulting to locale \"MessageBox_Title\"");
            }
            return MessageBox.Show(text, caption, buttons, icon, defaultButton, options, displayHelpButton);
        }
    }
}
