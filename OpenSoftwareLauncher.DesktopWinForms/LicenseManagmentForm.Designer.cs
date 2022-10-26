namespace OpenSoftwareLauncher.DesktopWinForms
{
    partial class LicenseManagmentForm
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
            System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Activate_PastTense", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup4 = new System.Windows.Forms.ListViewGroup("Diable_PastTense", System.Windows.Forms.HorizontalAlignment.Left);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LicenseManagmentForm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonCreateKeys = new System.Windows.Forms.ToolStripButton();
            this.treeViewGroups = new System.Windows.Forms.TreeView();
            this.listViewKeys = new System.Windows.Forms.ListView();
            this.columnHeaderUID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderCreated = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderExpiry = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderNote = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.propertyGridSelectedKey = new System.Windows.Forms.PropertyGrid();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.imageListKeyState = new System.Windows.Forms.ImageList(this.components);
            this.toolStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonCreateKeys});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1170, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonCreateKeys
            // 
            this.toolStripButtonCreateKeys.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonCreateKeys.Image = global::OpenSoftwareLauncher.DesktopWinForms.Properties.Resources.plus;
            this.toolStripButtonCreateKeys.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCreateKeys.Name = "toolStripButtonCreateKeys";
            this.toolStripButtonCreateKeys.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonCreateKeys.Text = "Create";
            this.toolStripButtonCreateKeys.Click += new System.EventHandler(this.toolStripButtonCreateKeys_Click);
            // 
            // treeViewGroups
            // 
            this.treeViewGroups.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewGroups.Location = new System.Drawing.Point(3, 16);
            this.treeViewGroups.MinimumSize = new System.Drawing.Size(150, 50);
            this.treeViewGroups.Name = "treeViewGroups";
            this.treeViewGroups.Size = new System.Drawing.Size(150, 406);
            this.treeViewGroups.TabIndex = 1;
            this.treeViewGroups.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewGroups_AfterSelect);
            // 
            // listViewKeys
            // 
            this.listViewKeys.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderUID,
            this.columnHeaderCreated,
            this.columnHeaderExpiry,
            this.columnHeaderNote});
            this.listViewKeys.Dock = System.Windows.Forms.DockStyle.Fill;
            listViewGroup3.Header = "Activate_PastTense";
            listViewGroup3.Name = "listViewGroup1";
            listViewGroup4.Header = "Diable_PastTense";
            listViewGroup4.Name = "listViewGroup2";
            this.listViewKeys.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup3,
            listViewGroup4});
            this.listViewKeys.HideSelection = false;
            this.listViewKeys.Location = new System.Drawing.Point(159, 16);
            this.listViewKeys.Name = "listViewKeys";
            this.listViewKeys.Size = new System.Drawing.Size(672, 406);
            this.listViewKeys.TabIndex = 2;
            this.listViewKeys.UseCompatibleStateImageBehavior = false;
            this.listViewKeys.View = System.Windows.Forms.View.Details;
            // 
            // columnHeaderUID
            // 
            this.columnHeaderUID.Text = "UID";
            this.columnHeaderUID.Width = 120;
            // 
            // columnHeaderCreated
            // 
            this.columnHeaderCreated.Text = "Created";
            this.columnHeaderCreated.Width = 170;
            // 
            // columnHeaderExpiry
            // 
            this.columnHeaderExpiry.Text = "Activation Expiry";
            this.columnHeaderExpiry.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeaderExpiry.Width = 170;
            // 
            // columnHeaderNote
            // 
            this.columnHeaderNote.Text = "Note";
            this.columnHeaderNote.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeaderNote.Width = 200;
            // 
            // propertyGridSelectedKey
            // 
            this.propertyGridSelectedKey.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGridSelectedKey.Location = new System.Drawing.Point(837, 16);
            this.propertyGridSelectedKey.Name = "propertyGridSelectedKey";
            this.propertyGridSelectedKey.Size = new System.Drawing.Size(330, 406);
            this.propertyGridSelectedKey.TabIndex = 3;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 66.92222F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.07778F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.treeViewGroups, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.listViewKeys, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.propertyGridSelectedKey, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 25);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1170, 425);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Group_Plural";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(159, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Key_Plural";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(837, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Detail_Plural";
            // 
            // imageListKeyState
            // 
            this.imageListKeyState.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListKeyState.ImageStream")));
            this.imageListKeyState.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListKeyState.Images.SetKeyName(0, "tick.ico");
            this.imageListKeyState.Images.SetKeyName(1, "fire.ico");
            this.imageListKeyState.Images.SetKeyName(2, "cross.ico");
            // 
            // LicenseManagmentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1170, 450);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(1186, 489);
            this.Name = "LicenseManagmentForm";
            this.Text = "LicenseManager";
            this.Shown += new System.EventHandler(this.LicenseManagmentForm_Shown);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonCreateKeys;
        private System.Windows.Forms.TreeView treeViewGroups;
        private System.Windows.Forms.ListView listViewKeys;
        private System.Windows.Forms.ColumnHeader columnHeaderUID;
        private System.Windows.Forms.ColumnHeader columnHeaderCreated;
        private System.Windows.Forms.ColumnHeader columnHeaderExpiry;
        private System.Windows.Forms.ColumnHeader columnHeaderNote;
        private System.Windows.Forms.PropertyGrid propertyGridSelectedKey;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ImageList imageListKeyState;
    }
}