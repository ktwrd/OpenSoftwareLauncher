using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ClientContext());
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
