using kate.shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace OSLCommon
{
    public class SystemAnnouncementEntry
    {
        public string Message { get; set; }
        public long Timestamp { get; set; }
        public bool Active { get; set; }
        public SystemAnnouncementEntry()
        {
            Message = "";
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            Active = false;
        }
    }
    public class SystemAnnouncementSummary
    {
        public SystemAnnouncementEntry[] Entries { get; set; }
        public bool Active { get; set; }
        public SystemAnnouncementSummary()
        {
            Entries = Array.Empty<SystemAnnouncementEntry>();
            Active = false;
        }
    }
    public class SystemAnnouncement
    {
        public List<SystemAnnouncementEntry> Entries = new List<SystemAnnouncementEntry>();
        public bool Active = false;

        public SystemAnnouncementEntry GetLatest()
        {
            Entries = new List<SystemAnnouncementEntry>(Entries.OrderBy(o => o.Timestamp));
            long ts = 0;
            SystemAnnouncementEntry target = null;
            foreach (var item in Entries)
            {
                if (item.Timestamp > ts && item.Active)
                    target = item;
            }
            return target;
        }

        public SystemAnnouncementEntry Set(string content, bool active = false)
        {
            var entry = new SystemAnnouncementEntry()
            {
                Message = content,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Active = active
            };
            Entries.Add(entry);
            OnUpdate();
            return entry;
        }

        public event VoidDelegate Update;
        public void OnUpdate()
        {
            if (Update != null)
                Update?.Invoke();
        }

        public SystemAnnouncementSummary GetSummary()
        {
            SystemAnnouncementSummary instance = new SystemAnnouncementSummary()
            {
                Entries = this.Entries.OrderBy(o => o.Timestamp).ToArray(),
                Active = this.Active
            };
            return instance;
        }
        public void Read(string content)
        {
            var serializerOptions = new JsonSerializerOptions()
            {
                IgnoreReadOnlyFields = false,
                IgnoreReadOnlyProperties = false,
                IncludeFields = true,
                WriteIndented = true
            };
            var summary = JsonSerializer.Deserialize<SystemAnnouncementSummary>(content, serializerOptions);
            if (summary != null)
            {
                Entries = new List<SystemAnnouncementEntry>(summary.Entries);
                Active = summary.Active;
            }
        }
        public string ToJSON()
        {
            var serializerOptions = new JsonSerializerOptions()
            {
                IgnoreReadOnlyFields = false,
                IgnoreReadOnlyProperties = false,
                IncludeFields = true,
                WriteIndented = true
            };
            return JsonSerializer.Serialize(GetSummary(), serializerOptions);
        }
    }
}
