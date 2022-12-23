using kate.shared.Extensions;
using OSLCommon.Licensing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using static OpenSoftwareLauncher.DesktopWinForms.LicenseManagmentForm;

namespace OpenSoftwareLauncher.DesktopWinForms
{
    public partial class LicenseManagmentForm : Form
    {
        public LicenseManagmentForm()
        {
            InitializeComponent();
            Locale();
        }
        public void Locale()
        {
            labelGroups.Text = LocaleManager.Get("Group_Plural");
            labelKeys.Text = LocaleManager.Get("Key_Plural");
            labelDetails.Text = LocaleManager.Get("Detail_Plural");
            toolStripDropDownButtonFilter.Text = LocaleManager.Get(toolStripDropDownButtonFilter.Text);
            toolStripDropDownButtonFilter.ToolTipText = toolStripDropDownButtonFilter.Text;

            foreach (ListViewGroup group in listViewKeys.Groups)
            {
                group.Header = LocaleManager.Get(group.Header);
            }

            Text = LocaleManager.Get("Title_LicenseManagement");
        }
        public void RefreshControls()
        {
            Program.LocalContent.PullLicenseKeys().Wait();

            ReloadGroupList();
            ReloadKeyList();
            RedrawProps();
        }
        public void ReloadGroupList()
        {
            var groupedLicences = new Dictionary<string, List<LicenseKeyMetadata>>();
            foreach (var itm in Program.LocalContent.LicenseKeyList)
            {
                if (!groupedLicences.ContainsKey(itm.GroupId))
                    groupedLicences.Add(itm.GroupId, new List<LicenseKeyMetadata>());
                groupedLicences[itm.GroupId].Add(itm);
            }

            treeViewGroups.Nodes.Clear();
            // Generate Nodes
            foreach (var pair in groupedLicences)
            {
                var parentNode = new TreeNode(pair.Key + " (" + pair.Value.Count + ")");
                parentNode.Name = pair.Key;
                var productList = new Dictionary<string, int>();
                foreach (var prod in pair.Value)
                {
                    foreach (var str in prod.Products)
                    {
                        if (!productList.ContainsKey(str))
                            productList.Add(str, 0);
                        productList[str]++;
                    }
                }

                foreach (var prodPair in productList)
                {
                    var nd = new TreeNode($"{prodPair.Key} ({prodPair.Value})");
                    nd.Name = $"{pair.Key}\n{prodPair.Key}";
                    parentNode.Nodes.Add(nd);
                }

                treeViewGroups.Nodes.Add(parentNode);
            }
            RedrawProps();
        }
        public enum eFilterAction
        {
            None,
            Group,
            Product,
            GroupAndProduct,
            LicenseKey
        }
        public string TargetGroup = "";
        public string TargetProduct = "";
        public string TargetLicenseKey = "";
        public eFilterAction FilterAction = eFilterAction.None;
        public void ReloadKeyList()
        {
            listViewKeys.Items.Clear();
            foreach (var key in Program.LocalContent.LicenseKeyList)
            {
                if (FilterAction == eFilterAction.LicenseKey && key.Key != TargetLicenseKey)
                    continue;
                else if (FilterAction == eFilterAction.GroupAndProduct)
                    if (key.GroupId != TargetGroup || !key.Products.Contains(TargetGroup))
                        continue;
                else if (FilterAction == eFilterAction.Group && key.GroupId != TargetGroup)
                    continue;
                else if (FilterAction == eFilterAction.Product && !key.Products.Contains(TargetProduct))
                    continue;
                var node = new ListViewItem(new String[]
                {
                    key.UID,
                    Program.Epoch.AddMilliseconds(key.CreatedTimestamp).ToString(),
                    Program.Epoch.AddMilliseconds(key.ActivateByTimestamp).ToString(),
                    key.InternalNote
                });
                node.Name = key.UID;
                if (!key.Enable)
                    node.ImageIndex = 2;
                if (key.ActivateByTimestamp > DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())
                    node.ImageIndex = 1;
                if (!key.Enable)
                    node.Group = listViewKeys.Groups[1];
                else if (key.Activated)
                    node.Group = listViewKeys.Groups[0];
                listViewKeys.Items.Add(node);
            }
            RedrawProps();
        }
        private void toolStripButtonCreateKeys_Click(object sender, EventArgs e)
        {
            var form = new LicenseKeyCreateDialog();
            form.MdiParent = MdiParent;
            form.Show();
            form.Closed += delegate
            {
                RefreshControls();
            };
        }

        private void LicenseManagmentForm_Shown(object sender, EventArgs e)
        {
            RefreshControls();
        }

        private void treeViewGroups_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var selected = treeViewGroups.SelectedNode;
            var splitted = selected.Name.Split(new String[]{ "\n" }, StringSplitOptions.None);
            if (splitted.Length < 2)
            {
                FilterAction = eFilterAction.Group;
                TargetGroup = selected.Name;
            }
            if (selected.Name.Contains("\n"))
            {
                FilterAction = eFilterAction.GroupAndProduct;
                TargetGroup = splitted[0];
                TargetProduct = splitted[1];
            }
            ReloadKeyList();
        }

        public LicenseKeyMetadata SelectedLicenseKey = null;

        private void listViewKeys_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedLicenseKey = null;
            if (listViewKeys.SelectedItems.Count > 0 && listViewKeys.SelectedItems.Count < 2)
            {
                string targetUID = ((ListViewItem)listViewKeys.SelectedItems[0]).Name;
                foreach (var item in Program.LocalContent.LicenseKeyList)
                {
                    if (item.UID == targetUID)
                    {
                        SelectedLicenseKey = item;
                        break;
                    }
                }
            }
            RedrawProps();
        }

        public void RedrawProps()
        {
            propertyGridSelectedKey.SelectedObject = JsonSerializer.Deserialize<LicenseKeyMetadata>(JsonSerializer.Serialize(SelectedLicenseKey, Program.serializerOptions), Program.serializerOptions);

            toolStripButtonKeyEnable.Enabled = listViewKeys.SelectedItems.Count > 0;
            toolStripButtonKeyDisable.Enabled = listViewKeys.SelectedItems.Count > 0;
        }

        private void toolStripButtonKeyEnable_Click(object sender, EventArgs e)
        {
            var taskList = new List<Task>();
            foreach (ListViewItem item in listViewKeys.SelectedItems)
            {
                taskList.Add(new Task(delegate
                {
                    Program.Client.EnableLicenseKey(item.Name).Wait();
                }));
            }
            foreach (var i in taskList)
                i.Start();
            Task.WhenAll(taskList).Wait();
            RefreshControls();
        }

        private void toolStripButtonKeyDisable_Click(object sender, EventArgs e)
        {
            var taskList = new List<Task>();
            foreach (ListViewItem item in listViewKeys.SelectedItems)
            {
                taskList.Add(new Task(delegate
                {
                    Program.Client.DisableLicenseKey(item.Name).Wait();
                }));
            }
            foreach (var i in taskList)
                i.Start();
            Task.WhenAll(taskList).Wait();
            RefreshControls();
        }
        private void byLicenseKeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new TextBoxForm(MessageBoxButtons.OK, true, "Enter license key", "License Key Filter");
            form.MdiParent = MdiParent;
            form.Done += (action) =>
            {
                if (action == DialogResult.OK)
                {
                    FilterAction = eFilterAction.LicenseKey;
                    TargetLicenseKey = form.textBoxContent.Text;
                    System.Threading.Thread.Sleep(100);
                    ReloadKeyList();
                }
            };
            form.Show();
        }

        private void noneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FilterAction = eFilterAction.None;
            ReloadKeyList();
        }
    }
}
