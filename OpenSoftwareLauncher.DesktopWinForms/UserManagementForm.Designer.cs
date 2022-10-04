﻿namespace OpenSoftwareLauncher.DesktopWinForms
{
    partial class UserManagementForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserManagementForm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.listViewAccounts = new System.Windows.Forms.ListView();
            this.columnHeaderUsername = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderEnabled = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderGroups = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderPermissions = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderLastSeen = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolStripButtonEdit = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonBanTool = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonPermissionTool = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonGroupTool = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonGroupEditorTool = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonEdit,
            this.toolStripSeparator1,
            this.toolStripButtonRefresh,
            this.toolStripSeparator2,
            this.toolStripButtonBanTool,
            this.toolStripButtonPermissionTool,
            this.toolStripButtonGroupTool,
            this.toolStripButtonGroupEditorTool});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(924, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // listViewAccounts
            // 
            this.listViewAccounts.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderUsername,
            this.columnHeaderEnabled,
            this.columnHeaderGroups,
            this.columnHeaderPermissions,
            this.columnHeaderLastSeen});
            this.listViewAccounts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewAccounts.HideSelection = false;
            this.listViewAccounts.Location = new System.Drawing.Point(0, 25);
            this.listViewAccounts.Name = "listViewAccounts";
            this.listViewAccounts.Size = new System.Drawing.Size(924, 436);
            this.listViewAccounts.TabIndex = 1;
            this.listViewAccounts.UseCompatibleStateImageBehavior = false;
            this.listViewAccounts.View = System.Windows.Forms.View.Details;
            // 
            // columnHeaderUsername
            // 
            this.columnHeaderUsername.Text = "FieldUsername";
            this.columnHeaderUsername.Width = 220;
            // 
            // columnHeaderEnabled
            // 
            this.columnHeaderEnabled.Text = "FieldEnabled";
            this.columnHeaderEnabled.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeaderEnabled.Width = 90;
            // 
            // columnHeaderGroups
            // 
            this.columnHeaderGroups.Text = "FieldGroupPlural";
            this.columnHeaderGroups.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeaderGroups.Width = 233;
            // 
            // columnHeaderPermissions
            // 
            this.columnHeaderPermissions.Text = "FieldPermissionPlural";
            this.columnHeaderPermissions.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeaderPermissions.Width = 233;
            // 
            // columnHeaderLastSeen
            // 
            this.columnHeaderLastSeen.Text = "FieldLastSeen";
            this.columnHeaderLastSeen.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeaderLastSeen.Width = 140;
            // 
            // toolStripButtonEdit
            // 
            this.toolStripButtonEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonEdit.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonEdit.Image")));
            this.toolStripButtonEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonEdit.Name = "toolStripButtonEdit";
            this.toolStripButtonEdit.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonEdit.Text = "FieldEdit";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonRefresh
            // 
            this.toolStripButtonRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRefresh.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonRefresh.Image")));
            this.toolStripButtonRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRefresh.Name = "toolStripButtonRefresh";
            this.toolStripButtonRefresh.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonRefresh.Text = "FieldRefresh";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonBanTool
            // 
            this.toolStripButtonBanTool.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonBanTool.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonBanTool.Image")));
            this.toolStripButtonBanTool.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonBanTool.Name = "toolStripButtonBanTool";
            this.toolStripButtonBanTool.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonBanTool.Text = "FieldBanish";
            // 
            // toolStripButtonPermissionTool
            // 
            this.toolStripButtonPermissionTool.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPermissionTool.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonPermissionTool.Image")));
            this.toolStripButtonPermissionTool.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPermissionTool.Name = "toolStripButtonPermissionTool";
            this.toolStripButtonPermissionTool.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonPermissionTool.Text = "FieldPermissionPlural";
            // 
            // toolStripButtonGroupTool
            // 
            this.toolStripButtonGroupTool.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonGroupTool.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonGroupTool.Image")));
            this.toolStripButtonGroupTool.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonGroupTool.Name = "toolStripButtonGroupTool";
            this.toolStripButtonGroupTool.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonGroupTool.Text = "FieldGroupPlural";
            // 
            // toolStripButtonGroupEditorTool
            // 
            this.toolStripButtonGroupEditorTool.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonGroupEditorTool.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonGroupEditorTool.Image")));
            this.toolStripButtonGroupEditorTool.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonGroupEditorTool.Name = "toolStripButtonGroupEditorTool";
            this.toolStripButtonGroupEditorTool.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonGroupEditorTool.Text = "FieldGroupUtility";
            // 
            // UserManagementForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(924, 461);
            this.Controls.Add(this.listViewAccounts);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(940, 500);
            this.Name = "UserManagementForm";
            this.Text = "WindowTitle_UserManagement";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ListView listViewAccounts;
        private System.Windows.Forms.ColumnHeader columnHeaderUsername;
        private System.Windows.Forms.ColumnHeader columnHeaderEnabled;
        private System.Windows.Forms.ColumnHeader columnHeaderGroups;
        private System.Windows.Forms.ColumnHeader columnHeaderPermissions;
        private System.Windows.Forms.ColumnHeader columnHeaderLastSeen;
        private System.Windows.Forms.ToolStripButton toolStripButtonEdit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonRefresh;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButtonBanTool;
        private System.Windows.Forms.ToolStripButton toolStripButtonPermissionTool;
        private System.Windows.Forms.ToolStripButton toolStripButtonGroupTool;
        private System.Windows.Forms.ToolStripButton toolStripButtonGroupEditorTool;
    }
}