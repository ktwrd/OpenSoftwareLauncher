namespace OpenSoftwareLauncher.DesktopWinForms
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Enable_PastTense", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("ServiceAccount", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Disable_PastTense", System.Windows.Forms.HorizontalAlignment.Left);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserManagementForm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonEdit = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonBanTool = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonUnban = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonPermissionTool = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonGroupTool = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonLicense = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonCreateServiceAccount = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonLicenseUtility = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonPermissionUtility = new System.Windows.Forms.ToolStripButton();
            this.listViewAccounts = new System.Windows.Forms.ListView();
            this.columnHeaderUsername = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderEnabled = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderGroups = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderPermissions = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderLastSeen = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonEdit,
            this.toolStripButtonDelete,
            this.toolStripSeparator1,
            this.toolStripButtonRefresh,
            this.toolStripSeparator2,
            this.toolStripButtonBanTool,
            this.toolStripButtonUnban,
            this.toolStripSeparator3,
            this.toolStripButtonPermissionTool,
            this.toolStripButtonGroupTool,
            this.toolStripButtonLicense,
            this.toolStripButtonCreateServiceAccount,
            this.toolStripButtonLicenseUtility,
            this.toolStripButtonPermissionUtility});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1029, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonEdit
            // 
            this.toolStripButtonEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonEdit.Enabled = false;
            this.toolStripButtonEdit.Image = global::OpenSoftwareLauncher.DesktopWinForms.Properties.Resources.pencil;
            this.toolStripButtonEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonEdit.Name = "toolStripButtonEdit";
            this.toolStripButtonEdit.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonEdit.Text = "Edit";
            // 
            // toolStripButtonDelete
            // 
            this.toolStripButtonDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonDelete.Enabled = false;
            this.toolStripButtonDelete.Image = global::OpenSoftwareLauncher.DesktopWinForms.Properties.Resources.minus;
            this.toolStripButtonDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDelete.Name = "toolStripButtonDelete";
            this.toolStripButtonDelete.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonDelete.Text = "Delete";
            this.toolStripButtonDelete.Click += new System.EventHandler(this.toolStripButtonDelete_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonRefresh
            // 
            this.toolStripButtonRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRefresh.Image = global::OpenSoftwareLauncher.DesktopWinForms.Properties.Resources.arrow_circle;
            this.toolStripButtonRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRefresh.Name = "toolStripButtonRefresh";
            this.toolStripButtonRefresh.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonRefresh.Text = "Refresh";
            this.toolStripButtonRefresh.Click += new System.EventHandler(this.toolStripButtonRefresh_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonBanTool
            // 
            this.toolStripButtonBanTool.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonBanTool.Enabled = false;
            this.toolStripButtonBanTool.Image = global::OpenSoftwareLauncher.DesktopWinForms.Properties.Resources.hammer;
            this.toolStripButtonBanTool.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonBanTool.Name = "toolStripButtonBanTool";
            this.toolStripButtonBanTool.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonBanTool.Text = "Banish";
            this.toolStripButtonBanTool.Click += new System.EventHandler(this.toolStripButtonBanTool_Click);
            // 
            // toolStripButtonUnban
            // 
            this.toolStripButtonUnban.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonUnban.Enabled = false;
            this.toolStripButtonUnban.Image = global::OpenSoftwareLauncher.DesktopWinForms.Properties.Resources.hammer__minus;
            this.toolStripButtonUnban.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonUnban.Name = "toolStripButtonUnban";
            this.toolStripButtonUnban.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonUnban.Text = "BanishReverse";
            this.toolStripButtonUnban.Click += new System.EventHandler(this.toolStripButtonUnban_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonPermissionTool
            // 
            this.toolStripButtonPermissionTool.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPermissionTool.Enabled = false;
            this.toolStripButtonPermissionTool.Image = global::OpenSoftwareLauncher.DesktopWinForms.Properties.Resources.key;
            this.toolStripButtonPermissionTool.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPermissionTool.Name = "toolStripButtonPermissionTool";
            this.toolStripButtonPermissionTool.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonPermissionTool.Text = "Permission_Plural";
            this.toolStripButtonPermissionTool.Click += new System.EventHandler(this.toolStripButtonPermissionTool_Click);
            // 
            // toolStripButtonGroupTool
            // 
            this.toolStripButtonGroupTool.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonGroupTool.Enabled = false;
            this.toolStripButtonGroupTool.Image = global::OpenSoftwareLauncher.DesktopWinForms.Properties.Resources.category_group_select;
            this.toolStripButtonGroupTool.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonGroupTool.Name = "toolStripButtonGroupTool";
            this.toolStripButtonGroupTool.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonGroupTool.Text = "Group_Plural";
            // 
            // toolStripButtonLicense
            // 
            this.toolStripButtonLicense.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonLicense.Enabled = false;
            this.toolStripButtonLicense.Image = global::OpenSoftwareLauncher.DesktopWinForms.Properties.Resources.briefcase;
            this.toolStripButtonLicense.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonLicense.Name = "toolStripButtonLicense";
            this.toolStripButtonLicense.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonLicense.Text = "LicenseUtility";
            this.toolStripButtonLicense.Click += new System.EventHandler(this.toolStripButtonLicense_Click);
            // 
            // toolStripButtonCreateServiceAccount
            // 
            this.toolStripButtonCreateServiceAccount.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonCreateServiceAccount.Enabled = false;
            this.toolStripButtonCreateServiceAccount.Image = global::OpenSoftwareLauncher.DesktopWinForms.Properties.Resources.hard_hat__plus;
            this.toolStripButtonCreateServiceAccount.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCreateServiceAccount.Name = "toolStripButtonCreateServiceAccount";
            this.toolStripButtonCreateServiceAccount.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonCreateServiceAccount.Text = "Create_ServiceAccount";
            this.toolStripButtonCreateServiceAccount.Click += new System.EventHandler(this.toolStripButtonCreateServiceAccount_Click);
            // 
            // toolStripButtonLicenseUtility
            // 
            this.toolStripButtonLicenseUtility.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonLicenseUtility.Image = global::OpenSoftwareLauncher.DesktopWinForms.Properties.Resources.license_key__arrow;
            this.toolStripButtonLicenseUtility.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonLicenseUtility.Name = "toolStripButtonLicenseUtility";
            this.toolStripButtonLicenseUtility.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonLicenseUtility.Text = "Title_BulkUserLicense";
            this.toolStripButtonLicenseUtility.Click += new System.EventHandler(this.toolStripButtonLicenseUtility_Click);
            // 
            // toolStripButtonPermissionUtility
            // 
            this.toolStripButtonPermissionUtility.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPermissionUtility.Image = global::OpenSoftwareLauncher.DesktopWinForms.Properties.Resources.key__arrow;
            this.toolStripButtonPermissionUtility.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPermissionUtility.Name = "toolStripButtonPermissionUtility";
            this.toolStripButtonPermissionUtility.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonPermissionUtility.Text = "Title_BulkUserPermission";
            this.toolStripButtonPermissionUtility.Click += new System.EventHandler(this.toolStripButtonPermissionUtility_Click);
            // 
            // listViewAccounts
            // 
            this.listViewAccounts.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listViewAccounts.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderUsername,
            this.columnHeaderEnabled,
            this.columnHeaderGroups,
            this.columnHeaderPermissions,
            this.columnHeaderLastSeen});
            this.listViewAccounts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewAccounts.FullRowSelect = true;
            this.listViewAccounts.GridLines = true;
            listViewGroup1.Header = "Enable_PastTense";
            listViewGroup1.Name = "listViewGroupDefault";
            listViewGroup2.Header = "ServiceAccount";
            listViewGroup2.Name = "listViewGroupServiceAccount";
            listViewGroup3.Header = "Disable_PastTense";
            listViewGroup3.Name = "listViewGroupDisabled";
            this.listViewAccounts.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3});
            this.listViewAccounts.HideSelection = false;
            this.listViewAccounts.LargeImageList = this.imageList1;
            this.listViewAccounts.Location = new System.Drawing.Point(0, 25);
            this.listViewAccounts.Name = "listViewAccounts";
            this.listViewAccounts.Size = new System.Drawing.Size(1029, 495);
            this.listViewAccounts.SmallImageList = this.imageList1;
            this.listViewAccounts.TabIndex = 1;
            this.listViewAccounts.UseCompatibleStateImageBehavior = false;
            this.listViewAccounts.View = System.Windows.Forms.View.Details;
            this.listViewAccounts.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listViewAccounts_ItemSelectionChanged);
            this.listViewAccounts.SelectedIndexChanged += new System.EventHandler(this.listViewAccounts_SelectedIndexChanged);
            this.listViewAccounts.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listViewAccounts_KeyUp);
            this.listViewAccounts.MouseUp += new System.Windows.Forms.MouseEventHandler(this.listViewAccounts_MouseUp);
            // 
            // columnHeaderUsername
            // 
            this.columnHeaderUsername.Text = "Username";
            this.columnHeaderUsername.Width = 220;
            // 
            // columnHeaderEnabled
            // 
            this.columnHeaderEnabled.Text = "Enable_PastTense";
            this.columnHeaderEnabled.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeaderEnabled.Width = 90;
            // 
            // columnHeaderGroups
            // 
            this.columnHeaderGroups.Text = "License_Plural";
            this.columnHeaderGroups.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeaderGroups.Width = 233;
            // 
            // columnHeaderPermissions
            // 
            this.columnHeaderPermissions.Text = "Permission_Plural";
            this.columnHeaderPermissions.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeaderPermissions.Width = 233;
            // 
            // columnHeaderLastSeen
            // 
            this.columnHeaderLastSeen.Text = "LastSeen";
            this.columnHeaderLastSeen.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeaderLastSeen.Width = 140;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "hard-hat.png");
            this.imageList1.Images.SetKeyName(1, "cross.png");
            // 
            // UserManagementForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1029, 520);
            this.Controls.Add(this.listViewAccounts);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(940, 500);
            this.Name = "UserManagementForm";
            this.Text = "Title_UserManagement";
            this.Shown += new System.EventHandler(this.UserManagementForm_Shown);
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
        private System.Windows.Forms.ToolStripButton toolStripButtonUnban;
        private System.Windows.Forms.ToolStripButton toolStripButtonLicense;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStripButton toolStripButtonCreateServiceAccount;
        private System.Windows.Forms.ToolStripButton toolStripButtonDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripButtonLicenseUtility;
        private System.Windows.Forms.ToolStripButton toolStripButtonPermissionUtility;
    }
}