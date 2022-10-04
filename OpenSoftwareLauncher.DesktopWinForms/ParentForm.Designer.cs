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
            this.toolStripButtonReleases = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonSettings = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonLogs = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Left;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonUsers,
            this.toolStripButtonAnnouncements,
            this.toolStripButtonReleases,
            this.toolStripSeparator1,
            this.toolStripButtonSettings,
            this.toolStripButtonLogs});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(32, 861);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonUsers
            // 
            this.toolStripButtonUsers.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonUsers.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonUsers.Image")));
            this.toolStripButtonUsers.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonUsers.Name = "toolStripButtonUsers";
            this.toolStripButtonUsers.Size = new System.Drawing.Size(29, 20);
            this.toolStripButtonUsers.Text = "WindowTitle_UserManagement";
            // 
            // toolStripButtonAnnouncements
            // 
            this.toolStripButtonAnnouncements.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAnnouncements.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAnnouncements.Image")));
            this.toolStripButtonAnnouncements.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAnnouncements.Name = "toolStripButtonAnnouncements";
            this.toolStripButtonAnnouncements.Size = new System.Drawing.Size(29, 20);
            this.toolStripButtonAnnouncements.Text = "FieldAnnouncementPlural";
            // 
            // toolStripButtonReleases
            // 
            this.toolStripButtonReleases.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonReleases.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonReleases.Image")));
            this.toolStripButtonReleases.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonReleases.Name = "toolStripButtonReleases";
            this.toolStripButtonReleases.Size = new System.Drawing.Size(29, 20);
            this.toolStripButtonReleases.Text = "FieldPackagePlural";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(29, 6);
            // 
            // toolStripButtonSettings
            // 
            this.toolStripButtonSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSettings.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSettings.Image")));
            this.toolStripButtonSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSettings.Name = "toolStripButtonSettings";
            this.toolStripButtonSettings.Size = new System.Drawing.Size(29, 20);
            this.toolStripButtonSettings.Text = "FieldSettingPlural";
            // 
            // toolStripButtonLogs
            // 
            this.toolStripButtonLogs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonLogs.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonLogs.Image")));
            this.toolStripButtonLogs.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonLogs.Name = "toolStripButtonLogs";
            this.toolStripButtonLogs.Size = new System.Drawing.Size(29, 20);
            this.toolStripButtonLogs.Text = "FieldLogPlural";
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
            this.Text = "WindowTitle_ParentForm";
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
        private System.Windows.Forms.ToolStripButton toolStripButtonReleases;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonSettings;
        private System.Windows.Forms.ToolStripButton toolStripButtonLogs;
    }
}