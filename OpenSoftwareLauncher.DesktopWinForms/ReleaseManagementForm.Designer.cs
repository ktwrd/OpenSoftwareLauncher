namespace OpenSoftwareLauncher.DesktopWinForms
{
    partial class ReleaseManagementForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReleaseManagementForm));
            this.treeViewBaseSignature = new System.Windows.Forms.TreeView();
            this.listViewStreamHistory = new System.Windows.Forms.ListView();
            this.columnHeaderHash = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderSignature = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderTimestamp = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButtonDelete = new System.Windows.Forms.ToolStripDropDownButton();
            this.remoteSignatureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButtonRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripDropDownButtonFilter = new System.Windows.Forms.ToolStripDropDownButton();
            this.showLatestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.toolStripButtonSave = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeViewBaseSignature
            // 
            this.treeViewBaseSignature.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewBaseSignature.Location = new System.Drawing.Point(0, 0);
            this.treeViewBaseSignature.Name = "treeViewBaseSignature";
            this.treeViewBaseSignature.Size = new System.Drawing.Size(156, 358);
            this.treeViewBaseSignature.TabIndex = 0;
            // 
            // listViewStreamHistory
            // 
            this.listViewStreamHistory.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderHash,
            this.columnHeaderSignature,
            this.columnHeaderTimestamp});
            this.listViewStreamHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewStreamHistory.FullRowSelect = true;
            this.listViewStreamHistory.GridLines = true;
            this.listViewStreamHistory.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewStreamHistory.HideSelection = false;
            this.listViewStreamHistory.Location = new System.Drawing.Point(0, 0);
            this.listViewStreamHistory.Name = "listViewStreamHistory";
            this.listViewStreamHistory.Size = new System.Drawing.Size(573, 358);
            this.listViewStreamHistory.TabIndex = 1;
            this.listViewStreamHistory.UseCompatibleStateImageBehavior = false;
            this.listViewStreamHistory.View = System.Windows.Forms.View.Details;
            this.listViewStreamHistory.SelectedIndexChanged += new System.EventHandler(this.listViewStreamHistory_SelectedIndexChanged);
            this.listViewStreamHistory.Click += new System.EventHandler(this.listViewStreamHistory_Click);
            // 
            // columnHeaderHash
            // 
            this.columnHeaderHash.Text = "Hash";
            this.columnHeaderHash.Width = 108;
            // 
            // columnHeaderSignature
            // 
            this.columnHeaderSignature.Text = "Signature";
            this.columnHeaderSignature.Width = 122;
            // 
            // columnHeaderTimestamp
            // 
            this.columnHeaderTimestamp.Text = "Timestamp";
            this.columnHeaderTimestamp.Width = 213;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButtonDelete,
            this.toolStripButtonRefresh,
            this.toolStripButtonSave,
            this.toolStripSeparator1,
            this.toolStripDropDownButtonFilter});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(733, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripDropDownButtonDelete
            // 
            this.toolStripDropDownButtonDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButtonDelete.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.remoteSignatureToolStripMenuItem});
            this.toolStripDropDownButtonDelete.Image = global::OpenSoftwareLauncher.DesktopWinForms.Properties.Resources.minus;
            this.toolStripDropDownButtonDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButtonDelete.Name = "toolStripDropDownButtonDelete";
            this.toolStripDropDownButtonDelete.Size = new System.Drawing.Size(29, 22);
            this.toolStripDropDownButtonDelete.Text = "Delete";
            this.toolStripDropDownButtonDelete.Click += new System.EventHandler(this.toolStripDropDownButtonDelete_Click);
            // 
            // remoteSignatureToolStripMenuItem
            // 
            this.remoteSignatureToolStripMenuItem.Image = global::OpenSoftwareLauncher.DesktopWinForms.Properties.Resources.geolocation;
            this.remoteSignatureToolStripMenuItem.Name = "remoteSignatureToolStripMenuItem";
            this.remoteSignatureToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.remoteSignatureToolStripMenuItem.Text = "RemoteSignature";
            // 
            // toolStripButtonRefresh
            // 
            this.toolStripButtonRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRefresh.Image = global::OpenSoftwareLauncher.DesktopWinForms.Properties.Resources.arrow_circle;
            this.toolStripButtonRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRefresh.Name = "toolStripButtonRefresh";
            this.toolStripButtonRefresh.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonRefresh.Text = "Refresh";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripDropDownButtonFilter
            // 
            this.toolStripDropDownButtonFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButtonFilter.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showLatestToolStripMenuItem});
            this.toolStripDropDownButtonFilter.Image = global::OpenSoftwareLauncher.DesktopWinForms.Properties.Resources.funnel;
            this.toolStripDropDownButtonFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButtonFilter.Name = "toolStripDropDownButtonFilter";
            this.toolStripDropDownButtonFilter.Size = new System.Drawing.Size(29, 22);
            this.toolStripDropDownButtonFilter.Text = "Filter";
            // 
            // showLatestToolStripMenuItem
            // 
            this.showLatestToolStripMenuItem.CheckOnClick = true;
            this.showLatestToolStripMenuItem.Name = "showLatestToolStripMenuItem";
            this.showLatestToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.showLatestToolStripMenuItem.Text = "ShowLatest";
            this.showLatestToolStripMenuItem.Click += new System.EventHandler(this.showLatestToolStripMenuItem_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeViewBaseSignature);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.listViewStreamHistory);
            this.splitContainer1.Size = new System.Drawing.Size(733, 358);
            this.splitContainer1.SplitterDistance = 156;
            this.splitContainer1.TabIndex = 3;
            // 
            // toolStripButtonSave
            // 
            this.toolStripButtonSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSave.Image = global::OpenSoftwareLauncher.DesktopWinForms.Properties.Resources.disk;
            this.toolStripButtonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSave.Name = "toolStripButtonSave";
            this.toolStripButtonSave.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonSave.Text = "Save";
            // 
            // ReleaseManagementForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(733, 383);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ReleaseManagementForm";
            this.Text = "Title_Release_Plural";
            this.Shown += new System.EventHandler(this.ReleaseManagementForm_Shown);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeViewBaseSignature;
        private System.Windows.Forms.ListView listViewStreamHistory;
        private System.Windows.Forms.ColumnHeader columnHeaderHash;
        private System.Windows.Forms.ColumnHeader columnHeaderSignature;
        private System.Windows.Forms.ColumnHeader columnHeaderTimestamp;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButtonDelete;
        private System.Windows.Forms.ToolStripMenuItem remoteSignatureToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButtonFilter;
        private System.Windows.Forms.ToolStripMenuItem showLatestToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButtonRefresh;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripButton toolStripButtonSave;
    }
}