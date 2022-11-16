﻿using kate.shared.Helpers;
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
        }

        public void Locale()
        {
            long start = OSLHelper.GetMicroseconds();
            toolStripButtonRefresh.Text = LocaleManager.Get("Refresh");
            toolStripDropDownButtonFilter.Text = LocaleManager.Get("Filter");
            foreach (ColumnHeader item in listView1.Columns)
            {
                item.Text = LocaleManager.Get(item.Text);
            }
            Text = LocaleManager.Get("Title_AuditLog");
            Trace.WriteLine($"[AuditLogForm->Locale] Took {OSLHelper.GetMicroseconds() - start}µs");
        }

        private void AuditLogForm_Shown(object sender, EventArgs e)
        {
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
        }

        public void RedrawElements()
        {
            Trace.WriteLine($"[DEBUG] [AuditLogForm->RedrawElements]");
            FilterItems();
            RedrawListView();
        }
        public void RedrawListView()
        {
            listView1.Items.Clear();
            foreach (var item in EntryListSorted)
            {
                var listViewItem = new ListViewItem(new string[]
                {
                    item.ActionType.ToString(),
                    item.Username,
                    DateTimeOffset.FromUnixTimeMilliseconds(item.Timestamp).LocalDateTime.ToString()
                });
                listViewItem.Name = item.UID;
                listView1.Items.Add(listViewItem);
            }
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

                Program.MessageBoxShow(LocaleManager.Get(exceptionDeserialized.Data.Message) + addon);
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
            List<AuditType> itemBlacklist = new List<AuditType>();
            for (int i = 0; i < checkedListBoxTypes.Items.Count; i++)
            {
                if (!checkedListBoxTypes.GetItemChecked(i))
                    itemBlacklist.Add((AuditType)checkedListBoxTypes.Items[i]);
            }

            var filtered = new List<AuditLogEntry>();
            var checkedItems = new List<AuditType>(checkedListBoxTypes.CheckedItems.Cast<AuditType>()).ToArray();
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
                    if (!checkedItems.Contains(item.ActionType))
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

        public AuditLogEntry[] EntryList = Array.Empty<AuditLogEntry>();
        public AuditLogEntry[] EntryListSorted = Array.Empty<AuditLogEntry>();
        public string[] UsernameFilter = Array.Empty<string>();

        private void checkedListBoxTypes_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            FilterItems();
        }

        private void toolStripButtonRefresh_Click(object sender, EventArgs e)
        {
            PullData();
        }
    }
}
