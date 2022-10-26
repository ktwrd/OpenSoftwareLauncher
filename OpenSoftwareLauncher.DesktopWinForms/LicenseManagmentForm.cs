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

namespace OpenSoftwareLauncher.DesktopWinForms
{
    public partial class LicenseManagmentForm : Form
    {
        public LicenseManagmentForm()
        {
            InitializeComponent();
        }
        public void RefreshControls()
        {
            Program.LocalContent.PullLicenseKeys().Wait();

            ReloadGroupList();
            ReloadKeyList();
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
        }
        public string FilterGroup = "";
        public string FilterProduct = "";
        public void ReloadKeyList()
        {
            listViewKeys.Items.Clear();
            foreach (var key in Program.LocalContent.LicenseKeyList)
            {
                if (FilterGroup.Length > 1 && key.GroupId != FilterGroup) continue;
                if (FilterProduct.Length > 1 && !key.Products.Contains(FilterProduct)) continue;
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
            FilterGroup = "";
            FilterProduct = "";
            if (splitted.Length < 2)
            {
                FilterGroup = selected.Name;
            }
            if (selected.Name.Contains("\n"))
            {
                FilterGroup = splitted[0];
                FilterProduct = splitted[1];
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
        }
    }
}
