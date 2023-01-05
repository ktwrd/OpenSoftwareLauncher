using kate.shared.Helpers;
using OSLCommon;
using OSLCommon.Logging;
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
    public partial class AuditLogForm : Form
    {
        public AuditLogForm()
        {
            InitializeComponent();
            TimestampMin = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + Program.Config.AuditLog.DefaultTimeRange_MinOffset;
        }

        public AuditLogEntry[] EntryList = Array.Empty<AuditLogEntry>();
        public AuditLogEntry[] EntryListSorted = Array.Empty<AuditLogEntry>();
        public string[] UsernameFilter = Array.Empty<string>();
        public long TimestampMin = long.MinValue;
        public long TimestampMax = long.MaxValue;
        public bool GroupByAction = false;
        public bool GroupByAccount = false;

        public void Locale()
        {
            toolStripButtonRefresh.Text = LocaleManager.Get("Refresh");
            toolStripButtonRefresh.ToolTipText = toolStripButtonRefresh.Text;
            
            toolStripDropDownButtonFilter.Text = LocaleManager.Get("Filter");
            toolStripDropDownButtonFilter.ToolTipText = toolStripDropDownButtonFilter.Text;

            timeRangeToolStripMenuItem.Text = LocaleManager.Get("TimeRange");
            timeRangeToolStripMenuItem.ToolTipText = timeRangeToolStripMenuItem.Text;

            groupByActionToolStripMenuItem.Text = LocaleManager.Get("GroupByAuditType");
            groupByActionToolStripMenuItem.ToolTipText = groupByActionToolStripMenuItem.Text;

            groupByAccountToolStripMenuItem.Text = LocaleManager.Get("GroupByAccount");
            groupByAccountToolStripMenuItem.ToolTipText = groupByAccountToolStripMenuItem.Text;

            foreach (ColumnHeader item in listView1.Columns)
            {
                item.Text = LocaleManager.Get(item.Text);
            }
            Text = LocaleManager.Get("Title_AuditLog");
        }

        private void AuditLogForm_Shown(object sender, EventArgs e)
        {
            long start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            Locale();
            var items = GeneralHelper.GetEnumList<AuditType>();
            var itemBlacklist = new AuditType[]
            {
                AuditType.Any
            };
            checkedListBoxTypes.Items.Clear();
            foreach (var entry in items)
            {
                if (itemBlacklist.Contains(entry))
                    continue;
                checkedListBoxTypes.Items.Add(entry);
            }
            for (int i = 0; i < checkedListBoxTypes.Items.Count; i++)
                checkedListBoxTypes.SetItemCheckState(i, CheckState.Checked);

            PullData();
            Trace.WriteLine($"[AuditLogForm->Shown] Took {DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - start}ms");
        }
        public void PopulateUsernameCheckedBoxList()
        {
            var accountList = EntryListSorted.Select(v => v.Username).Distinct().Where(v => v.Length > 0).ToList();
            accountList.Add("<None>");

            checkedListBoxUsers.Items.Clear();
            foreach (var username in accountList)
            {
                checkedListBoxUsers.Items.Add(username);
            }
            for (int i = 0; i < checkedListBoxUsers.Items.Count; i++)
            {
                checkedListBoxUsers.SetItemChecked(i, true);
            }
        }
        public void RedrawElements()
        {
            long start = OSLHelper.GetMicroseconds();
            Trace.WriteLine($"[AuditLogForm->RedrawElements] Start");
            FilterItems();
            PopulateUsernameCheckedBoxList();
            RedrawListView();
            Trace.WriteLine($"[AuditLogForm->RedrawElements] Took {OSLHelper.GetMicroseconds() - start}µs");
        }
        public void RedrawListView()
        {
            IgnoreListViewChange = true;
            listView1.Groups.Clear();
            if (GroupByAction)
            {
                var enumArray = GeneralHelper.GetEnumList<AuditType>();
                enumArray.Remove(AuditType.Any);

                foreach (var item in enumArray)
                {
                    var group = new ListViewGroup();
                    group.Header = item.ToString();
                    listView1.Groups.Add(group);
                }
            }
            else if (GroupByAccount)
            {
                var accountList = EntryListSorted.Select(v => v.Username).Distinct().Where(v => v.Length > 0).ToList();
                accountList.Add("<None>");
                foreach (var item in accountList)
                {
                    var group = new ListViewGroup();
                    group.Header = item;
                    listView1.Groups.Add(group);
                }
            }

            listView1.Items.Clear();
            foreach (var item in EntryListSorted)
            {
                if (item.Timestamp < TimestampMin || item.Timestamp > TimestampMax)
                    continue;

                string targetUsername = item.Username;
                if (targetUsername.Length < 1)
                    targetUsername = "<None>";
                var listViewItem = new ListViewItem(new string[]
                {
                    item.ActionType.ToString(),
                    item.Username,
                    DateTimeOffset.FromUnixTimeMilliseconds(item.Timestamp).LocalDateTime.ToString()
                });
                listViewItem.Name = item.UID;
                if (GroupByAction)
                {
                    listViewItem.Group = new List<ListViewGroup>(listView1.Groups.Cast<ListViewGroup>())
                        .Where(v => v.Header == item.ActionType.ToString())
                        .FirstOrDefault();
                }
                else if (GroupByAccount)
                {
                    listViewItem.Group = new List<ListViewGroup>(listView1.Groups.Cast<ListViewGroup>())
                        .Where(v => v.Header == targetUsername)
                        .FirstOrDefault();
                }
                if (checkedListBoxUsers.CheckedItems.Cast<string>().Contains(targetUsername))
                    listView1.Items.Add(listViewItem);
            }
            IgnoreListViewChange = false;
        }

        public void PullData()
        {
            var url = Endpoint.AuditLogGetAll(Program.Client.Token);
            var response = Program.Client.HttpClient.GetAsync(url).Result;
            var stringContent = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var deserialized = JsonSerializer.Deserialize<ObjectResponse<AuditLogEntry[]>>(stringContent, Program.serializerOptions);

                EntryList = deserialized.Data;
                RedrawElements();
            }
            else if ((int)response.StatusCode == 401)
            {
                var exceptionDeserialized = JsonSerializer.Deserialize<ObjectResponse<HttpException>>(stringContent, Program.serializerOptions);
                string addon = "";
                if (exceptionDeserialized.Data.Exception.Length > 0)
                    addon = "\n" + exceptionDeserialized.Data.Exception;
                
                Trace.WriteLine(string.Join("\n", new string[]
                {
                     "[AuditLogForm->PullData] Server responded with 401.",
                    $"    Message  : {exceptionDeserialized.Data.Message}",
                    $"    Exception: {exceptionDeserialized.Data.Exception}",
                    $"    Code     : {exceptionDeserialized.Data.Code}"
                }));
                new HttpExceptionModal(exceptionDeserialized.Data, (int)response.StatusCode, stringContent, url).Show();
                if (exceptionDeserialized.Data.Message == ServerStringResponse.InvalidPermission)
                {
                    Close();
                }
            }
            else
            {
                Trace.WriteLine(string.Join("\n", new string[]
                {
                    $"[AuditLogForm->PullData] Server responded with {(int)response.StatusCode} at {url}",
                     "======================== Content Start",
                     stringContent,
                     "======================== Content End"
                }));
                MessageBox.Show(
                    string.Join("\n", new string[]
                    {
                        $"URL: {url}",
                        $"Code: {(int)response.StatusCode}",
                        $"======== Content ========",
                        stringContent
                    }),
                    LocaleManager.Get("ServerResponse_Invalid"));
            }
        }

        public void FilterItems()
        {
            if (EntryList.Length < 1)
            {
                EntryListSorted = Array.Empty<AuditLogEntry>();
                return;
            }
            List<AuditType> itemBlacklist = new List<AuditType>();
            for (int i = 0; i < checkedListBoxTypes.Items.Count; i++)
            {
                if (!checkedListBoxTypes.GetItemChecked(i))
                    itemBlacklist.Add((AuditType)checkedListBoxTypes.Items[i]);
            }

            var filtered = new List<AuditLogEntry>();
            var checkedItemList = new List<AuditType>();
            for (int i = 0; i < checkedListBoxTypes.Items.Count; i++)
            {
                if (checkedListBoxTypes.GetItemChecked(i))
                    checkedItemList.Add((AuditType)checkedListBoxTypes.Items[i]);
            }
            var checkedItems = checkedItemList.ToArray();
            var uncheckedItems = new List<AuditType>(checkedListBoxTypes.Items.Cast<AuditType>())
                .Where(v => !checkedItems.Contains(v))
                .ToArray();
            foreach (var item in EntryList)
            {
                int min = 0;
                int max = 0;
                if (UsernameFilter.Length > 0)
                {
                    min++;
                    if (!UsernameFilter.Contains(item.Username))
                        max++;
                }
                if (uncheckedItems.Length > 0)
                {
                    min++;
                    if (checkedItems.Contains(item.ActionType))
                        max++;
                }

                if (min == max)
                    filtered.Add(item);
            }
            EntryListSorted = filtered
                .OrderByDescending(v => v.Timestamp)
                .ToArray();
            Trace.WriteLine($"[AuditLogForm->FilterItems] Diff Count: {EntryListSorted.Length - EntryList.Length}, Sorted Count: {EntryListSorted.Length}");
        }

        private void toolStripButtonRefresh_Click(object sender, EventArgs e)
        {
            PullData();
        }
        private bool IgnoreListViewChange = false;
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IgnoreListViewChange)
                return;
            object targetItem = null;
            if (listView1.SelectedItems.Count == 0)
            {
                propertyGrid1.SelectedObject = new object();
                propertyGrid1.ExpandAllGridItems();
                return;
            }
            if (listView1.SelectedItems.Count > 0)
            {
                string targetUID = listView1.SelectedItems[0].Name;
                foreach (var item in EntryList)
                {
                    if (targetUID == item.UID)
                    {
                        Type targetType = typeof(Dictionary<object, object[]>);

                        var typeDict = AuditLogManager.AuditTypeMap;

                        if (typeDict.ContainsKey(item.ActionType))
                            targetType = typeDict[item.ActionType];
                        
                        targetItem = JsonSerializer.Deserialize(item.ActionData, targetType, Program.serializerOptions);
                    }
                }
            }
            if (targetItem == null)
            {
                targetItem = new object();
            }
            TypeDescriptor.AddAttributes(targetItem, new Attribute[]
            {
                new ReadOnlyAttribute(true)
            });
            propertyGrid1.SelectedObject = targetItem;
            propertyGrid1.ExpandAllGridItems();
        }

        private void checkedListBoxTypes_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            return;
            FilterItems();
            RedrawListView();
        }

        private void timeRangeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new TimeRangeModal();
            form.MdiParent = MdiParent;
            if (TimestampMin > long.MinValue)
                form.Minimum = TimestampMin;
            if (TimestampMax < long.MaxValue)
                form.Maximum = TimestampMax;
            form.Complete += (min, max) =>
            {
                TimestampMin = min;
                TimestampMax = max;
                FilterItems();
                RedrawListView();
            };
            form.FormClosing += (o, f) =>
            {
                Enabled = true;
            };
            Enabled = false;
            form.Show();
        }

        private void groupByActionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GroupByAction = groupByActionToolStripMenuItem.Checked;
            groupByAccountToolStripMenuItem.Checked = false;
            GroupByAccount = false;
            FilterItems();
            RedrawListView();
        }

        private void groupByAccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GroupByAction = false;
            groupByActionToolStripMenuItem.Checked = false;
            GroupByAccount = groupByAccountToolStripMenuItem.Checked;
            FilterItems();
            RedrawListView();
        }

        private void checkedListBoxUsers_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            FilterItems();
            RedrawListView();
        }

        private void buttonCBoxSelect_All_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBoxUsers.Items.Count; i++)
            {
                checkedListBoxUsers.SetItemChecked(i, true);
            }
        }

        private void buttonCBoxSelect_Invert_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBoxUsers.Items.Count; i++)
            {
                checkedListBoxUsers.SetItemChecked(i, !checkedListBoxUsers.GetItemChecked(i));
            }
        }

        private void buttonCBoxSelect_All_Types_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBoxTypes.Items.Count; i++)
            {
                checkedListBoxTypes.SetItemChecked(i, true);
            }
        }

        private void buttonCBoxSelect_Invert_Types_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBoxTypes.Items.Count; i++)
            {
                checkedListBoxTypes.SetItemChecked(i, !checkedListBoxTypes.GetItemChecked(i));
            }
        }

        private void checkedListBoxTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterItems();
            RedrawListView();
        }

        private void checkedListBoxTypes_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ')
                checkedListBoxTypes_SelectedIndexChanged(null, new EventArgs());
        }
    }
}
