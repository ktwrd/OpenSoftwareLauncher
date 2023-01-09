using kate.shared.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenSoftwareLauncher.AdminClient
{
    public partial class CopyableErrorModal : Form
    {
        public CopyableErrorModal()
        {
            InitializeComponent();
        }
        public CopyableErrorModal(string content, string title="Error", VoidDelegate onClose=null)
            : this()
        {
            textBox1.Text = content;
            Text = title;
            if (onClose == null)
                return;
            FormClosed += (sender, e) =>
            {
                onClose?.Invoke();
            };
        }
        public CopyableErrorModal(string[] lines, string title="Error", VoidDelegate onClose = null)
            : this()
        {
            textBox1.Lines = lines;
            Text = title;
            if (onClose == null)
                return;
            FormClosed += (sender, e) =>
            {
                onClose?.Invoke();
            };
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CopyableErrorModal_Shown(object sender, EventArgs e)
        {
            Focus();
        }
    }
}
