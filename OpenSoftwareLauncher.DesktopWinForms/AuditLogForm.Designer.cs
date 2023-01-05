namespace OpenSoftwareLauncher.DesktopWinForms
{
    partial class AuditLogForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AuditLogForm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeaderType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderUsername = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderTimestamp = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.checkedListBoxTypes = new System.Windows.Forms.CheckedListBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.checkedListBoxUsers = new System.Windows.Forms.CheckedListBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.buttonCBoxSelect_All_Types = new System.Windows.Forms.Button();
            this.buttonCBoxSelect_Invert_Types = new System.Windows.Forms.Button();
            this.buttonCBoxSelect_All = new System.Windows.Forms.Button();
            this.buttonCBoxSelect_Invert = new System.Windows.Forms.Button();
            this.toolStripButtonRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStripDropDownButtonFilter = new System.Windows.Forms.ToolStripDropDownButton();
            this.timeRangeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupByActionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupByAccountToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonRefresh,
            this.toolStripSeparator1,
            this.toolStripDropDownButtonFilter});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1202, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // listView1
            // 
            this.listView1.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderType,
            this.columnHeaderUsername,
            this.columnHeaderTimestamp});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(527, 541);
            this.listView1.TabIndex = 1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // columnHeaderType
            // 
            this.columnHeaderType.Text = "Type";
            this.columnHeaderType.Width = 130;
            // 
            // columnHeaderUsername
            // 
            this.columnHeaderUsername.Text = "Username";
            this.columnHeaderUsername.Width = 180;
            // 
            // columnHeaderTimestamp
            // 
            this.columnHeaderTimestamp.Text = "Timestamp";
            this.columnHeaderTimestamp.Width = 180;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.BackColor = System.Drawing.Color.White;
            this.propertyGrid1.CategoryForeColor = System.Drawing.Color.Black;
            this.propertyGrid1.CategorySplitterColor = System.Drawing.Color.White;
            this.propertyGrid1.CommandsBorderColor = System.Drawing.Color.Gray;
            this.propertyGrid1.CommandsForeColor = System.Drawing.Color.Black;
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.HelpBackColor = System.Drawing.Color.White;
            this.propertyGrid1.HelpBorderColor = System.Drawing.Color.Gray;
            this.propertyGrid1.HelpForeColor = System.Drawing.Color.Black;
            this.propertyGrid1.LineColor = System.Drawing.Color.White;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.SelectedItemWithFocusForeColor = System.Drawing.Color.White;
            this.propertyGrid1.Size = new System.Drawing.Size(478, 541);
            this.propertyGrid1.TabIndex = 2;
            this.propertyGrid1.ViewBackColor = System.Drawing.Color.White;
            this.propertyGrid1.ViewBorderColor = System.Drawing.Color.Gray;
            this.propertyGrid1.ViewForeColor = System.Drawing.Color.Black;
            // 
            // checkedListBoxTypes
            // 
            this.checkedListBoxTypes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkedListBoxTypes.FormattingEnabled = true;
            this.checkedListBoxTypes.Location = new System.Drawing.Point(3, 3);
            this.checkedListBoxTypes.Name = "checkedListBoxTypes";
            this.checkedListBoxTypes.Size = new System.Drawing.Size(175, 222);
            this.checkedListBoxTypes.TabIndex = 3;
            this.checkedListBoxTypes.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBoxTypes_ItemCheck);
            this.checkedListBoxTypes.SelectedIndexChanged += new System.EventHandler(this.checkedListBoxTypes_SelectedIndexChanged);
            this.checkedListBoxTypes.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.checkedListBoxTypes_KeyPress);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1202, 541);
            this.splitContainer1.SplitterDistance = 189;
            this.splitContainer1.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tableLayoutPanel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(189, 541);
            this.panel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel1, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(189, 541);
            this.tableLayoutPanel2.TabIndex = 6;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.checkedListBoxTypes, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.flowLayoutPanel2, 0, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(4, 4);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(3, 3, 3, 9);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(181, 257);
            this.tableLayoutPanel3.TabIndex = 7;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoScroll = true;
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel2.Controls.Add(this.buttonCBoxSelect_All_Types);
            this.flowLayoutPanel2.Controls.Add(this.buttonCBoxSelect_Invert_Types);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(0, 228);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel2.MinimumSize = new System.Drawing.Size(0, 24);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(181, 29);
            this.flowLayoutPanel2.TabIndex = 4;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.checkedListBoxUsers, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(4, 280);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(181, 257);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.buttonCBoxSelect_All);
            this.flowLayoutPanel1.Controls.Add(this.buttonCBoxSelect_Invert);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 227);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.MinimumSize = new System.Drawing.Size(183, 30);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(183, 30);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // checkedListBoxUsers
            // 
            this.checkedListBoxUsers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkedListBoxUsers.FormattingEnabled = true;
            this.checkedListBoxUsers.Location = new System.Drawing.Point(3, 3);
            this.checkedListBoxUsers.Name = "checkedListBoxUsers";
            this.checkedListBoxUsers.Size = new System.Drawing.Size(175, 221);
            this.checkedListBoxUsers.TabIndex = 4;
            this.checkedListBoxUsers.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBoxUsers_ItemCheck);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.listView1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.propertyGrid1);
            this.splitContainer2.Size = new System.Drawing.Size(1009, 541);
            this.splitContainer2.SplitterDistance = 527;
            this.splitContainer2.TabIndex = 6;
            // 
            // buttonCBoxSelect_All_Types
            // 
            this.buttonCBoxSelect_All_Types.Image = global::OpenSoftwareLauncher.DesktopWinForms.Properties.Resources.table_select_all;
            this.buttonCBoxSelect_All_Types.Location = new System.Drawing.Point(3, 3);
            this.buttonCBoxSelect_All_Types.Name = "buttonCBoxSelect_All_Types";
            this.buttonCBoxSelect_All_Types.Size = new System.Drawing.Size(23, 23);
            this.buttonCBoxSelect_All_Types.TabIndex = 1;
            this.buttonCBoxSelect_All_Types.UseVisualStyleBackColor = true;
            this.buttonCBoxSelect_All_Types.Click += new System.EventHandler(this.buttonCBoxSelect_All_Types_Click);
            // 
            // buttonCBoxSelect_Invert_Types
            // 
            this.buttonCBoxSelect_Invert_Types.Image = global::OpenSoftwareLauncher.DesktopWinForms.Properties.Resources.arrow_switch;
            this.buttonCBoxSelect_Invert_Types.Location = new System.Drawing.Point(32, 3);
            this.buttonCBoxSelect_Invert_Types.Name = "buttonCBoxSelect_Invert_Types";
            this.buttonCBoxSelect_Invert_Types.Size = new System.Drawing.Size(23, 23);
            this.buttonCBoxSelect_Invert_Types.TabIndex = 2;
            this.buttonCBoxSelect_Invert_Types.UseVisualStyleBackColor = true;
            this.buttonCBoxSelect_Invert_Types.Click += new System.EventHandler(this.buttonCBoxSelect_Invert_Types_Click);
            // 
            // buttonCBoxSelect_All
            // 
            this.buttonCBoxSelect_All.Image = global::OpenSoftwareLauncher.DesktopWinForms.Properties.Resources.table_select_all;
            this.buttonCBoxSelect_All.Location = new System.Drawing.Point(3, 3);
            this.buttonCBoxSelect_All.Name = "buttonCBoxSelect_All";
            this.buttonCBoxSelect_All.Size = new System.Drawing.Size(23, 23);
            this.buttonCBoxSelect_All.TabIndex = 0;
            this.buttonCBoxSelect_All.UseVisualStyleBackColor = true;
            this.buttonCBoxSelect_All.Click += new System.EventHandler(this.buttonCBoxSelect_All_Click);
            // 
            // buttonCBoxSelect_Invert
            // 
            this.buttonCBoxSelect_Invert.Image = global::OpenSoftwareLauncher.DesktopWinForms.Properties.Resources.arrow_switch;
            this.buttonCBoxSelect_Invert.Location = new System.Drawing.Point(32, 3);
            this.buttonCBoxSelect_Invert.Name = "buttonCBoxSelect_Invert";
            this.buttonCBoxSelect_Invert.Size = new System.Drawing.Size(23, 23);
            this.buttonCBoxSelect_Invert.TabIndex = 1;
            this.buttonCBoxSelect_Invert.UseVisualStyleBackColor = true;
            this.buttonCBoxSelect_Invert.Click += new System.EventHandler(this.buttonCBoxSelect_Invert_Click);
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
            // toolStripDropDownButtonFilter
            // 
            this.toolStripDropDownButtonFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButtonFilter.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.timeRangeToolStripMenuItem,
            this.groupByActionToolStripMenuItem,
            this.groupByAccountToolStripMenuItem});
            this.toolStripDropDownButtonFilter.Image = global::OpenSoftwareLauncher.DesktopWinForms.Properties.Resources.funnel;
            this.toolStripDropDownButtonFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButtonFilter.Name = "toolStripDropDownButtonFilter";
            this.toolStripDropDownButtonFilter.Size = new System.Drawing.Size(29, 22);
            this.toolStripDropDownButtonFilter.Text = "Filter";
            // 
            // timeRangeToolStripMenuItem
            // 
            this.timeRangeToolStripMenuItem.Image = global::OpenSoftwareLauncher.DesktopWinForms.Properties.Resources.clock_select;
            this.timeRangeToolStripMenuItem.Name = "timeRangeToolStripMenuItem";
            this.timeRangeToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.timeRangeToolStripMenuItem.Text = "TimeRange";
            this.timeRangeToolStripMenuItem.Click += new System.EventHandler(this.timeRangeToolStripMenuItem_Click);
            // 
            // groupByActionToolStripMenuItem
            // 
            this.groupByActionToolStripMenuItem.CheckOnClick = true;
            this.groupByActionToolStripMenuItem.Image = global::OpenSoftwareLauncher.DesktopWinForms.Properties.Resources.script;
            this.groupByActionToolStripMenuItem.Name = "groupByActionToolStripMenuItem";
            this.groupByActionToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.groupByActionToolStripMenuItem.Text = "GroupByAuditType";
            this.groupByActionToolStripMenuItem.Click += new System.EventHandler(this.groupByActionToolStripMenuItem_Click);
            // 
            // groupByAccountToolStripMenuItem
            // 
            this.groupByAccountToolStripMenuItem.CheckOnClick = true;
            this.groupByAccountToolStripMenuItem.Image = global::OpenSoftwareLauncher.DesktopWinForms.Properties.Resources.users;
            this.groupByAccountToolStripMenuItem.Name = "groupByAccountToolStripMenuItem";
            this.groupByAccountToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.groupByAccountToolStripMenuItem.Text = "GroupByAccount";
            this.groupByAccountToolStripMenuItem.Click += new System.EventHandler(this.groupByAccountToolStripMenuItem_Click);
            // 
            // AuditLogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1202, 566);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AuditLogForm";
            this.Text = "Title_AuditLog";
            this.Shown += new System.EventHandler(this.AuditLogForm_Shown);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonRefresh;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeaderType;
        private System.Windows.Forms.ColumnHeader columnHeaderUsername;
        private System.Windows.Forms.ColumnHeader columnHeaderTimestamp;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButtonFilter;
        private System.Windows.Forms.CheckedListBox checkedListBoxTypes;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ToolStripMenuItem timeRangeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem groupByActionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem groupByAccountToolStripMenuItem;
        private System.Windows.Forms.CheckedListBox checkedListBoxUsers;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button buttonCBoxSelect_All;
        private System.Windows.Forms.Button buttonCBoxSelect_Invert;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Button buttonCBoxSelect_All_Types;
        private System.Windows.Forms.Button buttonCBoxSelect_Invert_Types;
    }
}