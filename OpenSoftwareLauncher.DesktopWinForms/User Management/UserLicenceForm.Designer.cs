namespace OpenSoftwareLauncher.DesktopWinForms
{
    partial class UserLicenceForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserLicenceForm));
            this.comboBoxCurrentLicense = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonGrant = new System.Windows.Forms.Button();
            this.listBoxLicenses = new System.Windows.Forms.ListBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonRemoveLicense = new System.Windows.Forms.Button();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.labelEmail = new System.Windows.Forms.Label();
            this.buttonPush = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxCurrentLicense
            // 
            this.comboBoxCurrentLicense.Dock = System.Windows.Forms.DockStyle.Top;
            this.comboBoxCurrentLicense.FormattingEnabled = true;
            this.comboBoxCurrentLicense.Location = new System.Drawing.Point(3, 3);
            this.comboBoxCurrentLicense.Name = "comboBoxCurrentLicense";
            this.comboBoxCurrentLicense.Size = new System.Drawing.Size(388, 21);
            this.comboBoxCurrentLicense.TabIndex = 0;
            this.comboBoxCurrentLicense.SelectedIndexChanged += new System.EventHandler(this.comboBoxCurrentLicense_SelectedIndexChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.buttonGrant, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.comboBoxCurrentLicense, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 225);
            this.tableLayoutPanel1.MinimumSize = new System.Drawing.Size(200, 27);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(432, 27);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // buttonGrant
            // 
            this.buttonGrant.Enabled = false;
            this.buttonGrant.Image = global::OpenSoftwareLauncher.DesktopWinForms.Properties.Resources.plus;
            this.buttonGrant.Location = new System.Drawing.Point(397, 3);
            this.buttonGrant.Name = "buttonGrant";
            this.buttonGrant.Size = new System.Drawing.Size(32, 23);
            this.buttonGrant.TabIndex = 2;
            this.buttonGrant.UseVisualStyleBackColor = true;
            this.buttonGrant.Click += new System.EventHandler(this.buttonGrant_Click);
            // 
            // listBoxLicenses
            // 
            this.listBoxLicenses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxLicenses.FormattingEnabled = true;
            this.listBoxLicenses.Location = new System.Drawing.Point(3, 3);
            this.listBoxLicenses.Name = "listBoxLicenses";
            this.listBoxLicenses.Size = new System.Drawing.Size(309, 194);
            this.listBoxLicenses.TabIndex = 2;
            this.listBoxLicenses.SelectedIndexChanged += new System.EventHandler(this.listBoxLicenses_SelectedIndexChanged);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.listBoxLicenses, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel1, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(6, 19);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(432, 200);
            this.tableLayoutPanel2.TabIndex = 3;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.buttonRemoveLicense);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(318, 3);
            this.flowLayoutPanel1.MinimumSize = new System.Drawing.Size(105, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(111, 194);
            this.flowLayoutPanel1.TabIndex = 3;
            // 
            // buttonRemoveLicense
            // 
            this.buttonRemoveLicense.Image = global::OpenSoftwareLauncher.DesktopWinForms.Properties.Resources.minus;
            this.buttonRemoveLicense.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonRemoveLicense.Location = new System.Drawing.Point(3, 3);
            this.buttonRemoveLicense.Name = "buttonRemoveLicense";
            this.buttonRemoveLicense.Size = new System.Drawing.Size(105, 23);
            this.buttonRemoveLicense.TabIndex = 0;
            this.buttonRemoveLicense.Text = "Remove";
            this.buttonRemoveLicense.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonRemoveLicense.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel1, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.labelEmail, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.buttonPush, 0, 3);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.Padding = new System.Windows.Forms.Padding(3);
            this.tableLayoutPanel3.RowCount = 4;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(444, 287);
            this.tableLayoutPanel3.TabIndex = 4;
            // 
            // labelEmail
            // 
            this.labelEmail.AutoSize = true;
            this.labelEmail.Location = new System.Drawing.Point(6, 3);
            this.labelEmail.Name = "labelEmail";
            this.labelEmail.Size = new System.Drawing.Size(43, 13);
            this.labelEmail.TabIndex = 4;
            this.labelEmail.Text = "<email>";
            // 
            // buttonPush
            // 
            this.buttonPush.Image = global::OpenSoftwareLauncher.DesktopWinForms.Properties.Resources.upload_cloud;
            this.buttonPush.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonPush.Location = new System.Drawing.Point(6, 258);
            this.buttonPush.Name = "buttonPush";
            this.buttonPush.Size = new System.Drawing.Size(75, 23);
            this.buttonPush.TabIndex = 5;
            this.buttonPush.Text = "Push";
            this.buttonPush.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonPush.UseVisualStyleBackColor = true;
            this.buttonPush.Click += new System.EventHandler(this.buttonPush_Click);
            // 
            // UserLicenceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(444, 287);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(460, 326);
            this.Name = "UserLicenceForm";
            this.Text = "Licenses - jane.doe@example.com";
            this.Shown += new System.EventHandler(this.UserLicenceForm_Shown);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxCurrentLicense;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button buttonGrant;
        private System.Windows.Forms.ListBox listBoxLicenses;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button buttonRemoveLicense;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label labelEmail;
        private System.Windows.Forms.Button buttonPush;
    }
}