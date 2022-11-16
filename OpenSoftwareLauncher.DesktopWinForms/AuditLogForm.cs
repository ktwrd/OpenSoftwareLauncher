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
        }

        public void Locale()
        {
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
        }

        public void RedrawElements()
        {
            Trace.WriteLine($"[DEBUG] [AuditLogForm->RedrawElements]");
            RedrawCheckboxTypes();
        }
        public void RedrawCheckboxTypes()
        {
            var items = GeneralHelper.GetEnumList<AuditType>();
            var itemBlacklist = new AuditType[]
            {
                AuditType.None,
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
            {
                AuditType checkboxItem = (AuditType)checkedListBoxTypes.Items[i];
                checkedListBoxTypes.SetItemChecked(i, !TypeBlacklist.Contains(checkboxItem));
            }
            Trace.WriteLine($"[INFO] [AuditLogForm->RedrawCheckboxTypes] CheckedCount: {checkedListBoxTypes.CheckedItems.Count}");
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

        public void UpdateFilter()
        {
            var sorted = new List<AuditLogEntry>();
            foreach (var item in EntryList)
            {
                int min = 0;
                int curr = 0;
                if (UsernameFilter.Length > 0)
                {
                    min++;
                    if (UsernameFilter.Contains(item.Username))
                        curr++;
                }
                if (TypeBlacklist.Length > 0)
                {
                    min++;
                    if (TypeBlacklist.Contains(item.ActionType))
                        curr++;
                }

                if (min == curr)
                    sorted.Add(item);
            }
            EntryListSorted = sorted.ToArray();
            Trace.WriteLine($"[AuditLogForm->UpdateFilter] Diff Count: {EntryListSorted.Length - EntryList.Length}");
            RedrawListView();
        }

        public void UpdateTypeBlacklist()
        {
            var blacklist = new List<AuditType>();
            for (int i = 0; i < checkedListBoxTypes.Items.Count; i++)
            {
                var item = (AuditType)checkedListBoxTypes.Items[i];
                if (checkedListBoxTypes.GetItemCheckState(i) == CheckState.Unchecked)
                    blacklist.Add(item);
            }
            TypeBlacklist = blacklist.ToArray();
            Trace.WriteLine($"[AuditLogForm->UpdateTypeBlacklist] Diff Count: {TypeBlacklist.Length - checkedListBoxTypes.Items.Count}");
            UpdateFilter();
        }

        public AuditLogEntry[] EntryList = Array.Empty<AuditLogEntry>();
        public AuditLogEntry[] EntryListSorted = Array.Empty<AuditLogEntry>();
        public string[] UsernameFilter = Array.Empty<string>();
        public AuditType[] TypeBlacklist = Array.Empty<AuditType>();

        private void checkedListBoxTypes_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            UpdateTypeBlacklist();
        }

        private void toolStripButtonRefresh_Click(object sender, EventArgs e)
        {
            PullData();
        }
    }
}
