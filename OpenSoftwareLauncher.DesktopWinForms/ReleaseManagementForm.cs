﻿using kate.shared.Helpers;
using OpenSoftwareLauncher.DesktopWinForms.ServerBridge;
using OSLCommon;
using OSLCommon.AutoUpdater;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
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
            toolStripButtonRefresh_Click(null, null);
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
                    .Where(v => treeViewBaseSignature.SelectedNode == null ? false : v.appID == treeViewBaseSignature.SelectedNode.Text)
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

        public async Task<bool> Pull()
        {
            var url = Endpoint.Release_ReleaseInfo(Program.Client.Token);
            var response = await Program.Client.HttpClient.GetAsync(url);
            var stringContent = response.Content.ReadAsStringAsync().Result;
            switch ((int)response.StatusCode)
            {
                case 200:
                    var successDeser = JsonSerializer.Deserialize<ObjectResponse<ReleaseInfo[]>>(stringContent, Program.serializerOptions);
                    WorkingReleaseInfo = successDeser.Data.ToList();
                    return true;
                    break;
                case 401:
                    var failDeser = JsonSerializer.Deserialize<ObjectResponse<HttpException>>(stringContent, Program.serializerOptions);
                    Trace.WriteLine($"[ReleaseManagementForm.Pull] Failed. Code 401 at {url}\n================\n{failDeser.Data.Message}\n{failDeser.Data.Error}\n================");
                    MessageBox.Show($"Code: {(int)response.StatusCode}\n{LocaleManager.Get(failDeser.Data.Message)}\n{failDeser.Data.Error}", "Pull Failure");
                    return false;
                    break;
                default:
                    Trace.WriteLine($"[ReleaseManagementForm.Pull] Failed. Code {(int)response.StatusCode} at {url}\n================\n{stringContent}\n================");
                    MessageBox.Show($"Code: {(int)response.StatusCode}\nURL: {url}\n========================\n{stringContent}", "Pull Failure");
                    return false;
                    break;
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

        private async void toolStripDropDownButtonDelete_Click(object sender, EventArgs e)
        {
            var taskList = new List<Task>();
            foreach (var selected in SelectedReleases)
            {
                taskList.Add(RemoveRelease(selected));
            }
            await Task.WhenAll(taskList);
            RefreshReleaseListView();
            RefreshReleaseTree();
        }
        /// <param name="releaseInfo">Release to delete (by commit+signature)</param>
        /// <returns>Was it successful?</returns>
        public async Task<bool> RemoveRelease(ReleaseInfo releaseInfo)
        {
            var url = Endpoint.Release_CommitHash(Program.Client.Token, releaseInfo.commitHash, releaseInfo.remoteLocation);
            var response = await Program.Client.HttpClient.DeleteAsync(url);
            var stringContent = response.Content.ReadAsStringAsync().Result;
            switch ((int)response.StatusCode)
            {
                case 200:
                    var successDeser = JsonSerializer.Deserialize<ObjectResponse<long>>(stringContent, Program.serializerOptions);
                    WorkingReleaseInfo = WorkingReleaseInfo.Where(v => v != releaseInfo).ToList();
                    Trace.WriteLine($"[ReleaseManagementForm.RemoveRelease] Deleted {successDeser.Data} items");
                    return true;
                    break;
                case 401:
                    var failDeser = JsonSerializer.Deserialize<ObjectResponse<HttpException>>(stringContent, Program.serializerOptions);
                    Trace.WriteLine($"[ReleaseManagementForm.RemoveRelease] Failed. Code 401 at {url}\n================\n{failDeser.Data.Message}\n{failDeser.Data.Error}\n================");
                    MessageBox.Show($"Code: {(int)response.StatusCode}\n{LocaleManager.Get(failDeser.Data.Message)}\n{failDeser.Data.Error}", "RemoveRelease Failure");
                    return false;
                    break;
                default:
                    Trace.WriteLine($"[ReleaseManagementForm.RemoveRelease] Failed. Code {(int)response.StatusCode} at {url}\n================\n{stringContent}\n================");
                    MessageBox.Show($"Code: {(int)response.StatusCode}\nURL: {url}\n========================\n{stringContent}", "RemoveRelease Failure");
                    return false;
                    break;
            }
        }

        public async Task RemoveReleaseBySignature(string signature)
        {
            var url = Endpoint.Release_Signature(Program.Client.Token, signature);
            var response = await Program.Client.HttpClient.DeleteAsync(url);
            var stringContent = response.Content.ReadAsStringAsync().Result;

            switch ((int)response.StatusCode)
            {
                case 200:
                    var successDeser = JsonSerializer.Deserialize<ObjectResponse<long>>(stringContent, Program.serializerOptions);
                    WorkingReleaseInfo = WorkingReleaseInfo.Where(v => v.remoteLocation != signature).ToList();
                    Trace.WriteLine($"[ReleaseManagementForm.RemoveReleaseBySignature] Deleted {successDeser.Data} items");
                    break;
                case 401:
                    var failDeser = JsonSerializer.Deserialize<ObjectResponse<HttpException>>(stringContent, Program.serializerOptions);
                    Trace.WriteLine($"[ReleaseManagementForm.RemoveReleaseBySignature] Failed. Code 401 at {url}\n================\n{failDeser.Data.Message}\n{failDeser.Data.Error}\n================");
                    MessageBox.Show($"Code: {(int)response.StatusCode}\n{LocaleManager.Get(failDeser.Data.Message)}\n{failDeser.Data.Error}", "RemoveReleaseBySignature Failure");
                    break;
                default:
                    Trace.WriteLine($"[ReleaseManagementForm.RemoveReleaseBySignature] Failed. Code {(int)response.StatusCode} at {url}\n================\n{stringContent}\n================");
                    MessageBox.Show($"Code: {(int)response.StatusCode}\nURL: {url}\n========================\n{stringContent}", "RemoveReleaseBySignature Failure");
                    break;
            }
        }

        private async void toolStripButtonRefresh_Click(object sender, EventArgs e)
        {
            var refreshViews = await Pull();
            if (refreshViews)
            {
                RefreshReleaseListView();
                RefreshReleaseTree();
            }
        }

        private void treeViewBaseSignature_AfterSelect(object sender, TreeViewEventArgs e)
        {
            RefreshReleaseListView();
        }

        private async void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {
            var taskList = new List<Task>();
            foreach (var selected in SelectedReleases)
            {
                taskList.Add(RemoveRelease(selected));
            }
            await Task.WhenAll(taskList);
            RefreshReleaseListView();
            RefreshReleaseTree();
        }

        private async void remoteSignatureToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var taskList = new List<Task>();
            foreach (var selected in SelectedReleases)
            {
                taskList.Add(RemoveReleaseBySignature(selected.remoteLocation));
            }
            await Task.WhenAll(taskList);
            RefreshReleaseListView();
            RefreshReleaseTree();
        }
    }
}
