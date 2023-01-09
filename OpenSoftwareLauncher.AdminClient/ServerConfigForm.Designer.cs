namespace OpenSoftwareLauncher.AdminClient
{
    partial class ServerConfigForm
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
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.checkBoxAllowAdminOverride = new System.Windows.Forms.CheckBox();
            this.checkBoxAllowReadReleaseBypass = new System.Windows.Forms.CheckBox();
            this.checkBoxRequireAuthentication = new System.Windows.Forms.CheckBox();
            this.labelTextBoxDefaultSignatures = new OpenSoftwareLauncher.AdminClient.CustomComponents.LabelTextBox();
            this.labelTextBoxImmuneUsers = new OpenSoftwareLauncher.AdminClient.CustomComponents.LabelTextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel4 = new System.Windows.Forms.FlowLayoutPanel();
            this.checkBoxPrometheus = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.labelTextBox1 = new OpenSoftwareLauncher.AdminClient.CustomComponents.LabelTextBox();
            this.labelTextBox2 = new OpenSoftwareLauncher.AdminClient.CustomComponents.LabelTextBox();
            this.flowLayoutPanel5 = new System.Windows.Forms.FlowLayoutPanel();
            this.buttonPush = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.flowLayoutPanel4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.flowLayoutPanel5.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.groupBox1);
            this.flowLayoutPanel1.Controls.Add(this.groupBox3);
            this.flowLayoutPanel1.Controls.Add(this.groupBox2);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(607, 409);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.Controls.Add(this.flowLayoutPanel2);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(252, 204);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Security";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.Controls.Add(this.checkBoxAllowAdminOverride);
            this.flowLayoutPanel2.Controls.Add(this.checkBoxAllowReadReleaseBypass);
            this.flowLayoutPanel2.Controls.Add(this.checkBoxRequireAuthentication);
            this.flowLayoutPanel2.Controls.Add(this.labelTextBoxDefaultSignatures);
            this.flowLayoutPanel2.Controls.Add(this.labelTextBoxImmuneUsers);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 16);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(246, 185);
            this.flowLayoutPanel2.TabIndex = 0;
            // 
            // checkBoxAllowAdminOverride
            // 
            this.checkBoxAllowAdminOverride.AutoSize = true;
            this.checkBoxAllowAdminOverride.Location = new System.Drawing.Point(3, 3);
            this.checkBoxAllowAdminOverride.Name = "checkBoxAllowAdminOverride";
            this.checkBoxAllowAdminOverride.Size = new System.Drawing.Size(126, 17);
            this.checkBoxAllowAdminOverride.TabIndex = 2;
            this.checkBoxAllowAdminOverride.Text = "Allow Admin Override";
            this.checkBoxAllowAdminOverride.UseVisualStyleBackColor = true;
            // 
            // checkBoxAllowReadReleaseBypass
            // 
            this.checkBoxAllowReadReleaseBypass.AutoSize = true;
            this.checkBoxAllowReadReleaseBypass.Location = new System.Drawing.Point(3, 26);
            this.checkBoxAllowReadReleaseBypass.Name = "checkBoxAllowReadReleaseBypass";
            this.checkBoxAllowReadReleaseBypass.Size = new System.Drawing.Size(224, 17);
            this.checkBoxAllowReadReleaseBypass.TabIndex = 3;
            this.checkBoxAllowReadReleaseBypass.Text = "Enable \"ReadReleaseBypass\" Permission";
            this.checkBoxAllowReadReleaseBypass.UseVisualStyleBackColor = true;
            // 
            // checkBoxRequireAuthentication
            // 
            this.checkBoxRequireAuthentication.AutoSize = true;
            this.checkBoxRequireAuthentication.Location = new System.Drawing.Point(3, 49);
            this.checkBoxRequireAuthentication.Name = "checkBoxRequireAuthentication";
            this.checkBoxRequireAuthentication.Size = new System.Drawing.Size(134, 17);
            this.checkBoxRequireAuthentication.TabIndex = 2;
            this.checkBoxRequireAuthentication.Text = "Require Authentication";
            this.checkBoxRequireAuthentication.UseVisualStyleBackColor = true;
            // 
            // labelTextBoxDefaultSignatures
            // 
            this.labelTextBoxDefaultSignatures.AutoSize = true;
            this.labelTextBoxDefaultSignatures.LabelText = "Default Signatures";
            this.labelTextBoxDefaultSignatures.Location = new System.Drawing.Point(3, 72);
            this.labelTextBoxDefaultSignatures.MinimumSize = new System.Drawing.Size(240, 52);
            this.labelTextBoxDefaultSignatures.MultiLine = true;
            this.labelTextBoxDefaultSignatures.Name = "labelTextBoxDefaultSignatures";
            this.labelTextBoxDefaultSignatures.Size = new System.Drawing.Size(240, 52);
            this.labelTextBoxDefaultSignatures.TabIndex = 2;
            // 
            // labelTextBoxImmuneUsers
            // 
            this.labelTextBoxImmuneUsers.AutoSize = true;
            this.labelTextBoxImmuneUsers.LabelText = "Immune Users";
            this.labelTextBoxImmuneUsers.Location = new System.Drawing.Point(3, 130);
            this.labelTextBoxImmuneUsers.MinimumSize = new System.Drawing.Size(240, 52);
            this.labelTextBoxImmuneUsers.MultiLine = true;
            this.labelTextBoxImmuneUsers.Name = "labelTextBoxImmuneUsers";
            this.labelTextBoxImmuneUsers.Size = new System.Drawing.Size(240, 52);
            this.labelTextBoxImmuneUsers.TabIndex = 4;
            // 
            // groupBox3
            // 
            this.groupBox3.AutoSize = true;
            this.groupBox3.Controls.Add(this.flowLayoutPanel4);
            this.groupBox3.Location = new System.Drawing.Point(261, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(94, 42);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Telemetry";
            // 
            // flowLayoutPanel4
            // 
            this.flowLayoutPanel4.AutoSize = true;
            this.flowLayoutPanel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel4.Controls.Add(this.checkBoxPrometheus);
            this.flowLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel4.Location = new System.Drawing.Point(3, 16);
            this.flowLayoutPanel4.Name = "flowLayoutPanel4";
            this.flowLayoutPanel4.Size = new System.Drawing.Size(88, 23);
            this.flowLayoutPanel4.TabIndex = 0;
            // 
            // checkBoxPrometheus
            // 
            this.checkBoxPrometheus.AutoSize = true;
            this.checkBoxPrometheus.Location = new System.Drawing.Point(3, 3);
            this.checkBoxPrometheus.Name = "checkBoxPrometheus";
            this.checkBoxPrometheus.Size = new System.Drawing.Size(82, 17);
            this.checkBoxPrometheus.TabIndex = 0;
            this.checkBoxPrometheus.Text = "Prometheus";
            this.checkBoxPrometheus.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.AutoSize = true;
            this.groupBox2.Controls.Add(this.flowLayoutPanel3);
            this.groupBox2.Location = new System.Drawing.Point(361, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(225, 83);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Authentication";
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.AutoSize = true;
            this.flowLayoutPanel3.Controls.Add(this.labelTextBox1);
            this.flowLayoutPanel3.Controls.Add(this.labelTextBox2);
            this.flowLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel3.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel3.Location = new System.Drawing.Point(3, 16);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(219, 64);
            this.flowLayoutPanel3.TabIndex = 1;
            // 
            // labelTextBox1
            // 
            this.labelTextBox1.AutoSize = true;
            this.labelTextBox1.LabelText = "Provider";
            this.labelTextBox1.Location = new System.Drawing.Point(3, 3);
            this.labelTextBox1.MinimumSize = new System.Drawing.Size(213, 26);
            this.labelTextBox1.MultiLine = false;
            this.labelTextBox1.Name = "labelTextBox1";
            this.labelTextBox1.Size = new System.Drawing.Size(213, 26);
            this.labelTextBox1.TabIndex = 2;
            // 
            // labelTextBox2
            // 
            this.labelTextBox2.AutoSize = true;
            this.labelTextBox2.LabelText = "Signup URL";
            this.labelTextBox2.Location = new System.Drawing.Point(3, 35);
            this.labelTextBox2.MinimumSize = new System.Drawing.Size(213, 26);
            this.labelTextBox2.MultiLine = false;
            this.labelTextBox2.Name = "labelTextBox2";
            this.labelTextBox2.Size = new System.Drawing.Size(213, 26);
            this.labelTextBox2.TabIndex = 3;
            // 
            // flowLayoutPanel5
            // 
            this.flowLayoutPanel5.AutoSize = true;
            this.flowLayoutPanel5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel5.Controls.Add(this.buttonPush);
            this.flowLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel5.Location = new System.Drawing.Point(3, 418);
            this.flowLayoutPanel5.Name = "flowLayoutPanel5";
            this.flowLayoutPanel5.Size = new System.Drawing.Size(607, 29);
            this.flowLayoutPanel5.TabIndex = 0;
            // 
            // buttonPush
            // 
            this.buttonPush.Location = new System.Drawing.Point(3, 3);
            this.buttonPush.Name = "buttonPush";
            this.buttonPush.Size = new System.Drawing.Size(75, 23);
            this.buttonPush.TabIndex = 0;
            this.buttonPush.Text = "Push";
            this.buttonPush.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel5, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(613, 450);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // ServerConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(613, 450);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ServerConfigForm";
            this.Text = "ServerConfigForm";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.flowLayoutPanel4.ResumeLayout(false);
            this.flowLayoutPanel4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
            this.flowLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.CheckBox checkBoxAllowAdminOverride;
        private System.Windows.Forms.CheckBox checkBoxAllowReadReleaseBypass;
        private System.Windows.Forms.CheckBox checkBoxRequireAuthentication;
        private CustomComponents.LabelTextBox labelTextBoxDefaultSignatures;
        private CustomComponents.LabelTextBox labelTextBoxImmuneUsers;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private CustomComponents.LabelTextBox labelTextBox1;
        private CustomComponents.LabelTextBox labelTextBox2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel4;
        private System.Windows.Forms.CheckBox checkBoxPrometheus;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel5;
        private System.Windows.Forms.Button buttonPush;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}