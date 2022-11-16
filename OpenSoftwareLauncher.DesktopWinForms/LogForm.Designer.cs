namespace OpenSoftwareLauncher.DesktopWinForms
{
    partial class LogForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogForm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonScrollToBottom = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabelLineCount = new System.Windows.Forms.ToolStripLabel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.scrollingRichTextBox1 = new OpenSoftwareLauncher.DesktopWinForms.CustomComponents.ScrollingRichTextBox();
            this.timerPendingContent = new System.Windows.Forms.Timer(this.components);
            this.toolStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonScrollToBottom,
            this.toolStripButtonSave,
            this.toolStripLabelLineCount});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(677, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonScrollToBottom
            // 
            this.toolStripButtonScrollToBottom.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonScrollToBottom.Image = global::OpenSoftwareLauncher.DesktopWinForms.Properties.Resources.arrow_270;
            this.toolStripButtonScrollToBottom.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonScrollToBottom.Name = "toolStripButtonScrollToBottom";
            this.toolStripButtonScrollToBottom.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonScrollToBottom.Text = "Btn_ScrollToBottom";
            this.toolStripButtonScrollToBottom.Click += new System.EventHandler(this.toolStripButtonScrollToBottom_Click);
            // 
            // toolStripButtonSave
            // 
            this.toolStripButtonSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSave.Image = global::OpenSoftwareLauncher.DesktopWinForms.Properties.Resources.disk;
            this.toolStripButtonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSave.Name = "toolStripButtonSave";
            this.toolStripButtonSave.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonSave.Text = "Save";
            this.toolStripButtonSave.Click += new System.EventHandler(this.toolStripButtonSave_Click);
            // 
            // toolStripLabelLineCount
            // 
            this.toolStripLabelLineCount.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripLabelLineCount.Name = "toolStripLabelLineCount";
            this.toolStripLabelLineCount.Size = new System.Drawing.Size(97, 22);
            this.toolStripLabelLineCount.Text = "LineCount_Plural";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.scrollingRichTextBox1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 25);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 443F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(677, 443);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // scrollingRichTextBox1
            // 
            this.scrollingRichTextBox1.BackColor = System.Drawing.Color.White;
            this.scrollingRichTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.scrollingRichTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scrollingRichTextBox1.Enabled = false;
            this.scrollingRichTextBox1.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.scrollingRichTextBox1.ForeColor = System.Drawing.Color.Black;
            this.scrollingRichTextBox1.Location = new System.Drawing.Point(3, 3);
            this.scrollingRichTextBox1.Name = "scrollingRichTextBox1";
            this.scrollingRichTextBox1.Size = new System.Drawing.Size(671, 437);
            this.scrollingRichTextBox1.TabIndex = 0;
            this.scrollingRichTextBox1.Text = "";
            // 
            // timerPendingContent
            // 
            this.timerPendingContent.Enabled = true;
            this.timerPendingContent.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // LogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(677, 468);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LogForm";
            this.Text = "Title_LogOutput";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LogForm_FormClosing);
            this.Shown += new System.EventHandler(this.LogForm_Shown);
            this.SizeChanged += new System.EventHandler(this.LogForm_SizeChanged);
            this.Enter += new System.EventHandler(this.LogForm_Enter);
            this.Leave += new System.EventHandler(this.LogForm_Leave);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ToolStripButton toolStripButtonScrollToBottom;
        private System.Windows.Forms.ToolStripButton toolStripButtonSave;
        private System.Windows.Forms.Timer timerPendingContent;
        private CustomComponents.ScrollingRichTextBox scrollingRichTextBox1;
        private System.Windows.Forms.ToolStripLabel toolStripLabelLineCount;
    }
}