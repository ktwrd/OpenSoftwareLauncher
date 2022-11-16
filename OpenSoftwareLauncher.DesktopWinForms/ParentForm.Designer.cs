namespace OpenSoftwareLauncher.DesktopWinForms
{
    partial class ParentForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ParentForm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonUsers = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAnnouncements = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonLicenceManagement = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonLicenseKeyCreator = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAuditLog = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Left;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonUsers,
            this.toolStripButtonAnnouncements,
            this.toolStripButtonLicenceManagement,
            this.toolStripButtonLicenseKeyCreator,
            this.toolStripButtonAuditLog});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0);
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(31, 861);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonUsers
            // 
            this.toolStripButtonUsers.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonUsers.Image = global::OpenSoftwareLauncher.DesktopWinForms.Properties.Resources.users;
            this.toolStripButtonUsers.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonUsers.Name = "toolStripButtonUsers";
            this.toolStripButtonUsers.Size = new System.Drawing.Size(30, 20);
            this.toolStripButtonUsers.Text = "Title_UserManagement";
            this.toolStripButtonUsers.Click += new System.EventHandler(this.toolStripButtonUsers_Click);
            // 
            // toolStripButtonAnnouncements
            // 
            this.toolStripButtonAnnouncements.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAnnouncements.Image = global::OpenSoftwareLauncher.DesktopWinForms.Properties.Resources.megaphone;
            this.toolStripButtonAnnouncements.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAnnouncements.Name = "toolStripButtonAnnouncements";
            this.toolStripButtonAnnouncements.Size = new System.Drawing.Size(30, 20);
            this.toolStripButtonAnnouncements.Text = "Announcement_Plural";
            this.toolStripButtonAnnouncements.Click += new System.EventHandler(this.toolStripButtonAnnouncements_Click);
            // 
            // toolStripButtonLicenceManagement
            // 
            this.toolStripButtonLicenceManagement.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonLicenceManagement.Image = global::OpenSoftwareLauncher.DesktopWinForms.Properties.Resources._lock;
            this.toolStripButtonLicenceManagement.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonLicenceManagement.Name = "toolStripButtonLicenceManagement";
            this.toolStripButtonLicenceManagement.Size = new System.Drawing.Size(30, 20);
            this.toolStripButtonLicenceManagement.Text = "Title_LicenseManagement";
            this.toolStripButtonLicenceManagement.Click += new System.EventHandler(this.toolStripButtonLicenceManagement_Click);
            // 
            // toolStripButtonLicenseKeyCreator
            // 
            this.toolStripButtonLicenseKeyCreator.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonLicenseKeyCreator.Image = global::OpenSoftwareLauncher.DesktopWinForms.Properties.Resources.lock__plus;
            this.toolStripButtonLicenseKeyCreator.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonLicenseKeyCreator.Name = "toolStripButtonLicenseKeyCreator";
            this.toolStripButtonLicenseKeyCreator.Size = new System.Drawing.Size(30, 20);
            this.toolStripButtonLicenseKeyCreator.Text = "License Key Creator";
            this.toolStripButtonLicenseKeyCreator.Click += new System.EventHandler(this.toolStripButtonLicenseKeyCreator_Click);
            // 
            // toolStripButtonAuditLog
            // 
            this.toolStripButtonAuditLog.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAuditLog.Image = global::OpenSoftwareLauncher.DesktopWinForms.Properties.Resources.blue_document_task;
            this.toolStripButtonAuditLog.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAuditLog.Name = "toolStripButtonAuditLog";
            this.toolStripButtonAuditLog.Size = new System.Drawing.Size(30, 20);
            this.toolStripButtonAuditLog.Text = "Title_AuditLog";
            this.toolStripButtonAuditLog.Click += new System.EventHandler(this.toolStripButtonAuditLog_Click);
            // 
            // ParentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1584, 861);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.Name = "ParentForm";
            this.Text = "Title_ParentForm";
            this.Shown += new System.EventHandler(this.ParentForm_Shown);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonUsers;
        private System.Windows.Forms.ToolStripButton toolStripButtonAnnouncements;
        private System.Windows.Forms.ToolStripButton toolStripButtonLicenseKeyCreator;
        private System.Windows.Forms.ToolStripButton toolStripButtonLicenceManagement;
        private System.Windows.Forms.ToolStripButton toolStripButtonAuditLog;
    }
}