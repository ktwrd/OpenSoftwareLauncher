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
    public partial class TextBoxForm : Form
    {
        public TextBoxForm()
        {
            InitializeComponent();
            Buttons = MessageBoxButtons.OK;
            EnableLabel = false;
        }
        public TextBoxForm(MessageBoxButtons buttons = MessageBoxButtons.OK, bool labelEnable = false, string label = "", string title = "")
        {
            InitializeComponent();
            Buttons = buttons;
            EnableLabel = labelEnable;
            Text = title;
            this.label.Text = label;
        }

        public void Locale()
        {
            // Only update locale of buttons
            FormHelper.LocaleControl(this.flowLayoutPanel1.Controls);
        }

        public delegate void DialogResultDelegate(DialogResult result);
        public event DialogResultDelegate Done;
        private bool HasDone = false;
        private void OnDone(DialogResult result)
        {
            if (Done != null)
                Done?.Invoke(result);
            HasDone = true;
            Close();
        }

        public bool EnableLabel
        {
            get
            {
                return tableLayoutPanel1.RowStyles[0].SizeType == SizeType.AutoSize;
            }
            set
            {
                if (value)
                {
                    tableLayoutPanel1.RowStyles[0].SizeType = SizeType.AutoSize;
                }
                else
                {
                    tableLayoutPanel1.RowStyles[0].SizeType = SizeType.Absolute;
                    tableLayoutPanel1.RowStyles[0].Height = 0;
                }
            }
        }
        private MessageBoxButtons _buttons = MessageBoxButtons.OK;
        public MessageBoxButtons Buttons
        {
            get => _buttons;
            set
            {
                _buttons = value;
                buttonCancel.Visible = false;
                buttonOk.Visible = false;
                buttonAbort.Visible = false;
                buttonRetry.Visible = false;
                buttonIgnore.Visible = false;
                buttonNo.Visible = false;
                buttonYes.Visible = false;
                switch (value)
                {
                    case MessageBoxButtons.OKCancel:
                        buttonCancel.Visible = true;
                        buttonOk.Visible = true;
                        break;
                    case MessageBoxButtons.OK:
                        buttonOk.Visible = true;
                        break;
                    case MessageBoxButtons.AbortRetryIgnore:
                        buttonAbort.Visible = true;
                        buttonRetry.Visible = true;
                        buttonIgnore.Visible = true;
                        break;
                    case MessageBoxButtons.YesNoCancel:
                        buttonCancel.Visible = true;
                        buttonNo.Visible = true;
                        buttonYes.Visible = true;
                        break;
                    case MessageBoxButtons.YesNo:
                        buttonNo.Visible = true;
                        buttonYes.Visible = true;
                        break;
                    case MessageBoxButtons.RetryCancel:
                        buttonCancel.Visible = true;
                        buttonRetry.Visible = true;
                        break;
                }
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            OnDone(DialogResult.Cancel);
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            OnDone(DialogResult.OK);
        }

        private void buttonIgnore_Click(object sender, EventArgs e)
        {
            OnDone(DialogResult.Ignore);
        }

        private void buttonRetry_Click(object sender, EventArgs e)
        {
            OnDone(DialogResult.Retry);
        }

        private void buttonAbort_Click(object sender, EventArgs e)
        {
            OnDone(DialogResult.Abort);
        }

        private void buttonNo_Click(object sender, EventArgs e)
        {
            OnDone(DialogResult.No);
        }

        private void buttonYes_Click(object sender, EventArgs e)
        {
            OnDone(DialogResult.Yes);
        }

        private void TextBoxForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!HasDone)
                OnDone(DialogResult.None);
        }

        private void TextBoxForm_Shown(object sender, EventArgs e)
        {
            Locale();
        }
    }
}
