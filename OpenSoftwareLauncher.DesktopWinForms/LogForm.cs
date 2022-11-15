﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenSoftwareLauncher.DesktopWinForms
{
    public partial class LogForm : Form
    {
        public LogForm()
        {
            InitializeComponent();
        }

        public void Locale()
        {
            toolStripButtonScrollToBottom.Text = LocaleManager.Get("Btn_ScrollToBottom");
            toolStripButtonSave.Text = LocaleManager.Get("Save");
            toolStripLabelLineCount.Text = "";
            Text = LocaleManager.Get("Title_LogOutput");
        }

        private void LogForm_Shown(object sender, EventArgs e)
        {
            Locale();
            timerPendingContent.Start();
            allow = true;
            scrollingRichTextBox1.ScrollToBottom();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (pendingContent && allow)
            {
                scrollingRichTextBox1.Text = string.Join("\n", contentToDoStuff.ToArray());
                var injected = new Dictionary<string, object>()
                {
                    {"count",  contentToDoStuff.Count}
                };
                toolStripLabelLineCount.Text = LocaleManager.Get("LineCount_Plural", inject: injected);
                scrollingRichTextBox1.ScrollToBottom();
                pendingContent = false;
            }
            timerPendingContent.Start();
        }

        private bool allow = false;
        private bool pendingContent = false;
        private List<string> contentToDoStuff = new List<string>();
        public void SetContent(string[] content)
        {
            if (!allow) return;
            if (!pendingContent)
            {
                pendingContent = true;
                contentToDoStuff = content.ToList();
            }
        }
        private void toolStripButtonScrollToBottom_Click(object sender, EventArgs e)
        {
            scrollingRichTextBox1.ScrollToBottom();
        }

        private void LogForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.MdiFormClosing)
                e.Cancel = true;
            else
                allow = false;
        }

        private void toolStripButtonSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Log File|*.log";
            dialog.Title = "Save log file";
            dialog.ShowDialog();

            if (dialog.FileName != "")
            {
                File.WriteAllLines(dialog.FileName, scrollingRichTextBox1.Lines);
            }
        }

        private void LogForm_SizeChanged(object sender, EventArgs e)
        {
            if (Size.Width < 50 || Size.Height < 50)
            {
                allow = false;
            }
            else
            {
                allow = true;
            }
        }

        private void LogForm_Leave(object sender, EventArgs e)
        {
            timerPendingContent.Interval = 2000;
        }

        private void LogForm_Enter(object sender, EventArgs e)
        {
            timerPendingContent.Interval = 100;
        }
    }
}
