using OSLCommon;
using OSLCommon.Authorization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenSoftwareLauncher.DesktopWinForms.ServerBridge
{
    public class LocalContent
    {
        public event ContentFieldDelegate OnPull;
        public event ContentFieldDelegate OnPullBefore;
        public event ContentFieldDelegate OnPush;
        public event ContentFieldDelegate OnPushBefore;

        internal Client Client => Program.Client;

        public Task Push()
        {
            if (Program.Client == null)
                return Task.CompletedTask;
            var taskList = new List<Task>()
            {
                new Task(delegate { PushAnnouncements(false).Wait();  }),
                new Task(delegate { PushContentManager(false).Wait(); })
            };

            foreach (var item in taskList)
                if (!item.IsCompleted)
                    item.Start();
            Task.WhenAll(taskList.ToArray()).Wait();

            return Task.CompletedTask;
        }
        public Task Pull()
        {
            if (Program.Client == null)
                return Task.CompletedTask;
            var taskList = new List<Task>()
            {
                new Task(delegate { PullAccounts(false).Wait(); }),
                new Task(delegate { PullAnnouncements(false).Wait(); }),
                new Task(delegate { PullContentManager(false).Wait(); })
            };

            foreach (var item in taskList)
                if (!item.IsCompleted)
                    item.Start();
            Task.WhenAll(taskList.ToArray()).Wait();

            return Task.CompletedTask;
        }

        #region Account
        public List<AccountDetailsResponse> AccountDetailList = new List<AccountDetailsResponse>();
        public Task PullAccounts(bool emit=true)
        {
            if (Program.Client == null) return Task.CompletedTask;
            AccountDetailList.Clear();
            if (!Client.HasPermission(AccountPermission.USER_LIST)) return Task.CompletedTask;
            var targetURL = Endpoint.UserList(Client.Token);
            var response = Client.HttpClient.GetAsync(targetURL).Result;
            var stringContent = response.Content.ReadAsStringAsync().Result;
            var dynamicContent = JsonSerializer.Deserialize<ObjectResponse<dynamic>>(stringContent, Program.serializerOptions);
            var content = JsonSerializer.Deserialize<ObjectResponse<AccountDetailsResponse[]>>(stringContent, Program.serializerOptions);
            if (!dynamicContent.Success || content == null)
            {
                Program.MessageBoxShow(stringContent, $"Failed to pull accounts");
                Trace.WriteLine($"[LocalContent->PullAccounts] Failed to fetch account listings\n--------\n{JsonSerializer.Serialize(dynamicContent, Program.serializerOptions)}\n--------\n");
                return Task.CompletedTask;
            }
            AccountDetailList = new List<AccountDetailsResponse>(content.Data);
            if (emit)
                OnPull?.Invoke(ContentField.Account);
            return Task.CompletedTask;
        }
        #endregion

        #region Announcement
        public SystemAnnouncementSummaryAsList AnnouncementSummary = new SystemAnnouncementSummaryAsList();
        public Task PullAnnouncements(bool emit = true)
        {
            AnnouncementSummary = new SystemAnnouncementSummaryAsList();
            if (!Client.HasPermission(AccountPermission.ANNOUNCEMENT_MANAGE))
                return Task.CompletedTask;
            var targetURL = Endpoint.AnnouncementSummary(Client.Token);
            Trace.WriteLine($"[LocalContent->RefreshAnnouncements] Fetching Response of {targetURL}");

            var response = Client.HttpClient.GetAsync(targetURL).Result;
            var stringContent = response.Content.ReadAsStringAsync().Result;
            var dynamicContent = JsonSerializer.Deserialize<ObjectResponse<dynamic>>(stringContent, Program.serializerOptions);
            var content = JsonSerializer.Deserialize<ObjectResponse<SystemAnnouncementSummaryAsList>>(stringContent, Program.serializerOptions);
            if (content == null || dynamicContent.Success == false)
            {
                MessageBox.Show($"{JsonSerializer.Serialize(dynamicContent, Program.serializerOptions)}", $"Failed to refresh announcements");
                Trace.WriteLine($"[LocalContent->RefreshAnnouncements] Failed to fetch announcements\n--------\n{JsonSerializer.Serialize(dynamicContent, Program.serializerOptions)}\n--------\n");
                return Task.CompletedTask;
            }

            AnnouncementSummary = content.Data;
            if (emit)
                OnPull?.Invoke(ContentField.Announcement);
            return Task.CompletedTask;
        }
        public Task PushAnnouncements(bool emit = true)
        {
            if (!Client.HasPermission(AccountPermission.ANNOUNCEMENT_MANAGE))
                return Task.CompletedTask;
            if (AnnouncementSummary == null)
            {
                Trace.WriteLine($"[LocalContent->PushAnnouncements] Cannot push since AnnouncementSummary is null.");
                MessageBox.Show($"AnnouncementSummary is null ;w;", $"Failed to push announcements");
                return Task.CompletedTask;
            }

            if (AnnouncementSummary.Entries.Count < 1)
                AnnouncementSummary.Active = false;
            int activeCount = 0;
            foreach (var item in AnnouncementSummary.Entries)
                if (item.Active)
                    activeCount++;
            if (activeCount < 1)
                AnnouncementSummary.Active = false;

            var arraySummary = new SystemAnnouncementSummary()
            {
                Active = AnnouncementSummary.Active,
                Entries = AnnouncementSummary.Entries.ToArray()
            };

            var targetURL = Endpoint.AnnouncementSetData(Client.Token, arraySummary);
            var response = Client.HttpClient.GetAsync(targetURL).Result;
            var stringContent = response.Content.ReadAsStringAsync().Result;
            var dynamicContent = JsonSerializer.Deserialize<ObjectResponse<dynamic>>(stringContent, Program.serializerOptions);
            var content = JsonSerializer.Deserialize<ObjectResponse<SystemAnnouncementSummary>>(stringContent, Program.serializerOptions);
            if (!dynamicContent.Success || content == null)
            {
                MessageBox.Show($"{stringContent}", $"Failed to push announcements");
                Trace.WriteLine($"[LocalContent->PushAnnouncements] Failed to push announcements\n--------\n{JsonSerializer.Serialize(dynamicContent, Program.serializerOptions)}\n--------\n");
                return Task.CompletedTask;
            }
            AnnouncementSummary = new SystemAnnouncementSummaryAsList()
            {
                Active = content.Data.Active,
                Entries = new List<SystemAnnouncementEntry>(content.Data.Entries)
            };
            if (emit)
                OnPush?.Invoke(ContentField.Announcement);
            return Task.CompletedTask;
        }
        #endregion

        #region Content Manager
        public AllDataResult ContentManagerAlias = null;
        public Task PullContentManager(bool emit = true)
        {
            ContentManagerAlias = null;
            if (!Client.HasPermission(AccountPermission.ADMINISTRATOR))
                return Task.CompletedTask;
            var targetURL = Endpoint.DumpDataFetch(Client.Token, DataType.All);
            var response = Client.HttpClient.GetAsync(targetURL).Result;
            var stringContent = response.Content.ReadAsStringAsync().Result;
            var dynamicContent = JsonSerializer.Deserialize<ObjectResponse<dynamic>>(stringContent, Program.serializerOptions);
            var content = JsonSerializer.Deserialize<ObjectResponse<AllDataResult>>(stringContent, Program.serializerOptions);
            if (!dynamicContent.Success || content == null)
            {
                MessageBox.Show($"{stringContent}", $"Failed to refresh content manager");
                Trace.WriteLine($"[LocalContent->PullContentManager] Failed to fetch content manager\n--------\n{JsonSerializer.Serialize(dynamicContent, Program.serializerOptions)}\n--------\n");

                return Task.CompletedTask;
            }
            ContentManagerAlias = content.Data;
            if (emit)
                OnPull?.Invoke(ContentField.ContentManager);
            return Task.CompletedTask;
        }
        public Task PushContentManager(bool emit = true)
        {
            if (!Client.HasPermission(AccountPermission.ADMINISTRATOR))
                return Task.CompletedTask;
            if (ContentManagerAlias == null)
            {
                Trace.WriteLine($"[LocalContent->PushContentManager] Cannot push since ContentManagerAlias is null");
                MessageBox.Show("ContentManagerAlias is null ;w;", "Failed to push Content Manager");
                return Task.CompletedTask;
            }

            ContentManagerAlias.Releases = ReleaseHelper.TransformReleaseList(ContentManagerAlias.ReleaseInfoContent.ToArray());

            var targetURL = Endpoint.DumpSetData(Client.Token, DataType.All);
            var pushContent = new ObjectResponse<AllDataResult>()
            {
                Success = true,
                Data = ContentManagerAlias
            };
            var _strcon = new StringContent(JsonSerializer.Serialize(pushContent, Program.serializerOptions));
            var response = Client.HttpClient.PostAsync(targetURL, _strcon).Result;
            var stringContent = response.Content.ReadAsStringAsync().Result;
            var dynamicContent = JsonSerializer.Deserialize<ObjectResponse<dynamic>>(stringContent, Program.serializerOptions);
            if (dynamicContent.Success == false)
            {
                MessageBox.Show($"{stringContent}", $"Failed to push content manager");
                Trace.WriteLine($"[LocalContent->PushContentManager] Failed to push content manager\n--------\n{JsonSerializer.Serialize(dynamicContent, Program.serializerOptions)}\n--------\n");
                return Task.CompletedTask;
            }
            var content = JsonSerializer.Deserialize<ObjectResponse<object>>(stringContent, Program.serializerOptions);
            if (!dynamicContent.Success || content == null)
            {
                MessageBox.Show($"{stringContent}", $"Failed to push content manager");
                Trace.WriteLine($"[LocalContent->PushContentManager] Failed to push content manager\n--------\n{JsonSerializer.Serialize(dynamicContent, Program.serializerOptions)}\n--------\n");
                return Task.CompletedTask;
            }
            var contentTyped = JsonSerializer.Deserialize<ObjectResponse<AllDataResult>>(stringContent, Program.serializerOptions);
            ContentManagerAlias = contentTyped.Data;
            if (emit)
                OnPush?.Invoke(ContentField.ContentManager);
            return Task.CompletedTask;
        }

        public string[] GetRemoteLocations()
        {
            var lst = new List<string>();
            foreach (var i in ContentManagerAlias.ReleaseInfoContent)
                if (!lst.Contains(i.remoteLocation))
                    lst.Add(i.remoteLocation);
            return lst.ToArray();
        }
        #endregion
    }
}
