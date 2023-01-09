namespace OpenSoftwareLauncher.AdminClient
{
    partial class TextBoxForm
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonIgnore = new System.Windows.Forms.Button();
            this.buttonRetry = new System.Windows.Forms.Button();
            this.buttonAbort = new System.Windows.Forms.Button();
            this.buttonNo = new System.Windows.Forms.Button();
            this.buttonYes = new System.Windows.Forms.Button();
            this.textBoxContent = new System.Windows.Forms.TextBox();
            this.label = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.textBoxContent, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.MinimumSize = new System.Drawing.Size(300, 80);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(302, 82);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.buttonCancel);
            this.flowLayoutPanel1.Controls.Add(this.buttonOk);
            this.flowLayoutPanel1.Controls.Add(this.buttonIgnore);
            this.flowLayoutPanel1.Controls.Add(this.buttonRetry);
            this.flowLayoutPanel1.Controls.Add(this.buttonAbort);
            this.flowLayoutPanel1.Controls.Add(this.buttonNo);
            this.flowLayoutPanel1.Controls.Add(this.buttonYes);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 51);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.MaximumSize = new System.Drawing.Size(0, 30);
            this.flowLayoutPanel1.MinimumSize = new System.Drawing.Size(0, 30);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(302, 30);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(224, 3);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Visible = false;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(143, 3);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 0;
            this.buttonOk.Text = "Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Visible = false;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonIgnore
            // 
            this.buttonIgnore.Location = new System.Drawing.Point(62, 3);
            this.buttonIgnore.Name = "buttonIgnore";
            this.buttonIgnore.Size = new System.Drawing.Size(75, 23);
            this.buttonIgnore.TabIndex = 2;
            this.buttonIgnore.Text = "Ignore";
            this.buttonIgnore.UseVisualStyleBackColor = true;
            this.buttonIgnore.Visible = false;
            this.buttonIgnore.Click += new System.EventHandler(this.buttonIgnore_Click);
            // 
            // buttonRetry
            // 
            this.buttonRetry.Location = new System.Drawing.Point(224, 32);
            this.buttonRetry.Name = "buttonRetry";
            this.buttonRetry.Size = new System.Drawing.Size(75, 23);
            this.buttonRetry.TabIndex = 3;
            this.buttonRetry.Text = "Retry";
            this.buttonRetry.UseVisualStyleBackColor = true;
            this.buttonRetry.Visible = false;
            this.buttonRetry.Click += new System.EventHandler(this.buttonRetry_Click);
            // 
            // buttonAbort
            // 
            this.buttonAbort.Location = new System.Drawing.Point(143, 32);
            this.buttonAbort.Name = "buttonAbort";
            this.buttonAbort.Size = new System.Drawing.Size(75, 23);
            this.buttonAbort.TabIndex = 4;
            this.buttonAbort.Text = "Abort";
            this.buttonAbort.UseVisualStyleBackColor = true;
            this.buttonAbort.Visible = false;
            this.buttonAbort.Click += new System.EventHandler(this.buttonAbort_Click);
            // 
            // buttonNo
            // 
            this.buttonNo.Location = new System.Drawing.Point(62, 32);
            this.buttonNo.Name = "buttonNo";
            this.buttonNo.Size = new System.Drawing.Size(75, 23);
            this.buttonNo.TabIndex = 5;
            this.buttonNo.Text = "No";
            this.buttonNo.UseVisualStyleBackColor = true;
            this.buttonNo.Visible = false;
            this.buttonNo.Click += new System.EventHandler(this.buttonNo_Click);
            // 
            // buttonYes
            // 
            this.buttonYes.Location = new System.Drawing.Point(224, 61);
            this.buttonYes.Name = "buttonYes";
            this.buttonYes.Size = new System.Drawing.Size(75, 23);
            this.buttonYes.TabIndex = 6;
            this.buttonYes.Text = "Yes";
            this.buttonYes.UseVisualStyleBackColor = true;
            this.buttonYes.Visible = false;
            this.buttonYes.Click += new System.EventHandler(this.buttonYes_Click);
            // 
            // textBoxContent
            // 
            this.textBoxContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxContent.Location = new System.Drawing.Point(3, 28);
            this.textBoxContent.MinimumSize = new System.Drawing.Size(4, 20);
            this.textBoxContent.Name = "textBoxContent";
            this.textBoxContent.Size = new System.Drawing.Size(296, 20);
            this.textBoxContent.TabIndex = 1;
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Location = new System.Drawing.Point(3, 3);
            this.label.Margin = new System.Windows.Forms.Padding(3);
            this.label.Name = "label";
            this.label.Padding = new System.Windows.Forms.Padding(3);
            this.label.Size = new System.Drawing.Size(35, 19);
            this.label.TabIndex = 2;
            this.label.Text = "label";
            // 
            // TextBoxForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(326, 106);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(332, 135);
            this.Name = "TextBoxForm";
            this.Padding = new System.Windows.Forms.Padding(12);
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TextBoxForm_FormClosing);
            this.Shown += new System.EventHandler(this.TextBoxForm_Shown);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        public System.Windows.Forms.Button buttonOk;
        public System.Windows.Forms.TextBox textBoxContent;
        public System.Windows.Forms.Label label;
        public System.Windows.Forms.Button buttonCancel;
        public System.Windows.Forms.Button buttonIgnore;
        public System.Windows.Forms.Button buttonRetry;
        public System.Windows.Forms.Button buttonAbort;
        public System.Windows.Forms.Button buttonNo;
        public System.Windows.Forms.Button buttonYes;
    }
}