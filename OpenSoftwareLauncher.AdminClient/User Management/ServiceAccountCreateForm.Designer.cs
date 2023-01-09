namespace OpenSoftwareLauncher.AdminClient
{
    partial class ServiceAccountCreateForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServiceAccountCreateForm));
            this.labelUsername = new System.Windows.Forms.Label();
            this.textBoxUsername = new System.Windows.Forms.TextBox();
            this.checkedListBoxPermissions = new System.Windows.Forms.CheckedListBox();
            this.checkedListBoxLicenses = new System.Windows.Forms.CheckedListBox();
            this.labelPermissions = new System.Windows.Forms.Label();
            this.labelLicenses = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonPush = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelUsername
            // 
            this.labelUsername.AutoSize = true;
            this.labelUsername.Location = new System.Drawing.Point(3, 3);
            this.labelUsername.Margin = new System.Windows.Forms.Padding(3);
            this.labelUsername.Name = "labelUsername";
            this.labelUsername.Padding = new System.Windows.Forms.Padding(3);
            this.labelUsername.Size = new System.Drawing.Size(61, 19);
            this.labelUsername.TabIndex = 1;
            this.labelUsername.Text = "Username";
            // 
            // textBoxUsername
            // 
            this.textBoxUsername.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBoxUsername.Location = new System.Drawing.Point(3, 28);
            this.textBoxUsername.MinimumSize = new System.Drawing.Size(200, 20);
            this.textBoxUsername.Name = "textBoxUsername";
            this.textBoxUsername.Size = new System.Drawing.Size(200, 20);
            this.textBoxUsername.TabIndex = 1;
            // 
            // checkedListBoxPermissions
            // 
            this.checkedListBoxPermissions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkedListBoxPermissions.FormattingEnabled = true;
            this.checkedListBoxPermissions.Location = new System.Drawing.Point(209, 28);
            this.checkedListBoxPermissions.Name = "checkedListBoxPermissions";
            this.checkedListBoxPermissions.Size = new System.Drawing.Size(198, 425);
            this.checkedListBoxPermissions.TabIndex = 1;
            // 
            // checkedListBoxLicenses
            // 
            this.checkedListBoxLicenses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkedListBoxLicenses.FormattingEnabled = true;
            this.checkedListBoxLicenses.Location = new System.Drawing.Point(413, 28);
            this.checkedListBoxLicenses.Name = "checkedListBoxLicenses";
            this.checkedListBoxLicenses.Size = new System.Drawing.Size(198, 425);
            this.checkedListBoxLicenses.TabIndex = 2;
            // 
            // labelPermissions
            // 
            this.labelPermissions.AutoSize = true;
            this.labelPermissions.Location = new System.Drawing.Point(209, 3);
            this.labelPermissions.Margin = new System.Windows.Forms.Padding(3);
            this.labelPermissions.Name = "labelPermissions";
            this.labelPermissions.Padding = new System.Windows.Forms.Padding(3);
            this.labelPermissions.Size = new System.Drawing.Size(95, 19);
            this.labelPermissions.TabIndex = 3;
            this.labelPermissions.Text = "Permission_Plural";
            // 
            // labelLicenses
            // 
            this.labelLicenses.AutoSize = true;
            this.labelLicenses.Location = new System.Drawing.Point(413, 3);
            this.labelLicenses.Margin = new System.Windows.Forms.Padding(3);
            this.labelLicenses.Name = "labelLicenses";
            this.labelLicenses.Padding = new System.Windows.Forms.Padding(3);
            this.labelLicenses.Size = new System.Drawing.Size(82, 19);
            this.labelLicenses.TabIndex = 4;
            this.labelLicenses.Text = "License_Plural";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.labelUsername, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelPermissions, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.checkedListBoxLicenses, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBoxUsername, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.checkedListBoxPermissions, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelLicenses, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(614, 456);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.buttonPush, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel1, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(620, 491);
            this.tableLayoutPanel2.TabIndex = 7;
            // 
            // buttonPush
            // 
            this.buttonPush.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPush.Image = global::OpenSoftwareLauncher.AdminClient.Properties.Resources.upload_cloud;
            this.buttonPush.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonPush.Location = new System.Drawing.Point(542, 465);
            this.buttonPush.Name = "buttonPush";
            this.buttonPush.Size = new System.Drawing.Size(75, 23);
            this.buttonPush.TabIndex = 5;
            this.buttonPush.Text = "Push";
            this.buttonPush.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonPush.UseVisualStyleBackColor = true;
            this.buttonPush.Click += new System.EventHandler(this.buttonPush_Click);
            // 
            // ServiceAccountCreateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(620, 491);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(636, 530);
            this.Name = "ServiceAccountCreateForm";
            this.Text = "Title_ServiceAccountCreate";
            this.Shown += new System.EventHandler(this.ServiceAccountCreateForm_Shown);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label labelUsername;
        private System.Windows.Forms.TextBox textBoxUsername;
        private System.Windows.Forms.CheckedListBox checkedListBoxPermissions;
        private System.Windows.Forms.CheckedListBox checkedListBoxLicenses;
        private System.Windows.Forms.Label labelPermissions;
        private System.Windows.Forms.Label labelLicenses;
        private System.Windows.Forms.Button buttonPush;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    }
}