using kate.shared.Helpers;
using OpenSoftwareLauncher.DesktopWinForms.ServerBridge;
using OSLCommon.AutoUpdater;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenSoftwareLauncher.DesktopWinForms
{
    public partial class ReleaseManagementForm : Form
    {
        public ReleaseManagementForm()
        {
            InitializeComponent();
        }

        public void Locale()
        {
            Text = LocaleManager.Get(Text);

            toolStripDropDownButtonDelete.Text = LocaleManager.Get(toolStripDropDownButtonDelete.Text);
            toolStripDropDownButtonDelete.ToolTipText = toolStripDropDownButtonDelete.Text;
            remoteSignatureToolStripMenuItem.Text = LocaleManager.Get(remoteSignatureToolStripMenuItem.Text);
            remoteSignatureToolStripMenuItem.ToolTipText = remoteSignatureToolStripMenuItem.Text;

            toolStripButtonRefresh.Text = LocaleManager.Get(toolStripButtonRefresh.Text);
            toolStripButtonRefresh.ToolTipText = toolStripButtonRefresh.Text;

            toolStripButtonSave.Text = LocaleManager.Get(toolStripButtonSave.Text);
            toolStripButtonSave.ToolTipText = toolStripButtonSave.Text;

            toolStripDropDownButtonFilter.Text = LocaleManager.Get(toolStripDropDownButtonFilter.Text);
            toolStripDropDownButtonFilter.ToolTipText = toolStripDropDownButtonFilter.Text;
            showLatestToolStripMenuItem.Text = LocaleManager.Get(showLatestToolStripMenuItem.Text);
            showLatestToolStripMenuItem.ToolTipText = showLatestToolStripMenuItem.Text;

            foreach (ColumnHeader column in listViewStreamHistory.Columns)
            {
                column.Text = LocaleManager.Get(column.Text);
            }
        }
        private void ReleaseManagementForm_Shown(object sender, EventArgs e)
        {
            Locale();
        }

        public List<ReleaseInfo> WorkingReleaseInfo = new List<ReleaseInfo>();

        public void RefreshReleaseTree()
        {
            treeViewBaseSignature.Nodes.Clear();
            Dictionary<string, List<ReleaseInfo>> releaseInfoDict = new Dictionary<string, List<ReleaseInfo>>();
            foreach (var release in WorkingReleaseInfo)
            {
                if (release.appID.Length < 1) continue;
                if (!releaseInfoDict.ContainsKey(release.appID))
                    releaseInfoDict.Add(release.appID, new List<ReleaseInfo>());
                releaseInfoDict[release.appID].Add(release);
            }
            foreach (var pair in releaseInfoDict)
            {
                treeViewBaseSignature.Nodes.Add($"{pair.Key}");
            }
            listViewStreamHistory_SelectedIndexChanged(null, null);
        }
        public void RefreshReleaseListView()
        {
            listViewStreamHistory.Items.Clear();
            var targetReleaseList = WorkingReleaseInfo
                    .Where(v => v.appID == treeViewBaseSignature.SelectedNode.Text)
                    .OrderByDescending(s => s.timestamp).ToList();

            if (UserConfig.GetBoolean("General", "ShowLatestRelease", true))
            {
                targetReleaseList = targetReleaseList.GroupBy(v => v.remoteLocation)
                    .Select(v => v.First()).ToList();
            }
            foreach (var item in targetReleaseList)
            {
                var lvitem = new ListViewItem(
                    new string[]
                    {
                        item.commitHashShort,
                        item.remoteLocation,
                        DateTimeOffset.FromUnixTimeMilliseconds(item.timestamp).ToString()
                    });
                lvitem.Name = WorkingReleaseInfo.IndexOf(item).ToString();
                listViewStreamHistory.Items.Add(lvitem);
            }
        }

        public List<ReleaseInfo> SelectedReleases = new List<ReleaseInfo>();
        public event VoidDelegate SelectedReleasesChange;
        private void listViewStreamHistory_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedReleases.Clear();
            for (int i = 0; i < listViewStreamHistory.SelectedItems.Count; i++)
            {
                var item = listViewStreamHistory.SelectedItems[i];
                int attemptedIndex = int.Parse(item.Name);
                if (attemptedIndex >= 0)
                {
                    SelectedReleases.Add(WorkingReleaseInfo[attemptedIndex]);
                }
            }
            if (SelectedReleasesChange != null)
                SelectedReleasesChange?.Invoke();
        }

        private void listViewStreamHistory_Click(object sender, EventArgs e)
        {
            listViewStreamHistory_SelectedIndexChanged(null, null);
        }

        private void showLatestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserConfig.Set("General", "ShowLatestRelease", showLatestToolStripMenuItem.Checked);
            RefreshReleaseListView();
        }

        private void toolStripDropDownButtonDelete_Click(object sender, EventArgs e)
        {
            foreach (var selected in SelectedReleases)
            {
                RemoveRelease(selected);
            }
            RefreshReleaseListView();
            RefreshReleaseTree();
        }
        public bool RemoveRelease(ReleaseInfo releaseInfo)
            => WorkingReleaseInfo.Remove(releaseInfo);

        public void RemoveReleaseBySignature(string signature)
        {
            WorkingReleaseInfo = WorkingReleaseInfo.Where(v => v.remoteLocation != signature).ToList();
        }
    }
}
