using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenSoftwareLauncher.AdminClient.CustomComponents
{
    public partial class LabelTextBox : UserControl
    {
        public LabelTextBox()
        {
            InitializeComponent();
            label1.Text = labelText;
            textBox1.Multiline = multiLine;
        }

        private string labelText;
        public string LabelText
        {
            get
            {
                return label1.Text;
            }
            set
            {
                labelText = value;
                label1.Text = labelText;
            }
        }
        private bool multiLine;
        public bool MultiLine
        {
            get
            {
                return textBox1.Multiline;
            }
            set
            {
                multiLine = value;
                textBox1.Multiline = multiLine;
            }
        }

        public Label Label => label1;
        public TextBox TextBox => textBox1;
    }
}
