using kate.shared.Helpers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OSLCommon
{
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
    public delegate void AnnouncementDelegate(SystemAnnouncementEntry announcement);

    public partial class SystemAnnouncement
    {
        public List<SystemAnnouncementEntry> Entries = new List<SystemAnnouncementEntry>();
        public bool Active = false;

        internal void InjectEntry(SystemAnnouncementEntry entry)
        {
            if (entry == null || entry.hooked) return;
            AnnouncementUpdate += (item) =>
            {
                if (item.ID == entry.ID)
                {
                    entry.Merge(item);
                }
            };
            entry.hooked = true;
        }
        public virtual SystemAnnouncementEntry GetLatest()
        {
            Entries = new List<SystemAnnouncementEntry>(Entries.OrderBy(o => o.Timestamp));
            long ts = 0;
            SystemAnnouncementEntry target = null;
            foreach (var item in Entries)
            {
                if (item.Timestamp > ts && item.Active)
                    target = item;
            }
            InjectEntry(target);
            return target;
        }
        public virtual SystemAnnouncementEntry[] GetAll()
        {
            return Entries.ToArray();
        }
        public virtual void Set(SystemAnnouncementEntry[] all)
        {
            var toUpdate = all.Where(t => Entries.Where(v => v.ID == t.ID).Count() == 1).ToList();
            var newThings = all.Where(t => Entries.Where(v => v.ID == t.ID).Count() < 1).ToList();

            foreach (var item in Entries)
            {
                foreach (var thing in toUpdate)
                {
                    item.Message = thing.Message;
                    item.Active = thing.Active;
                }
            }
            foreach (var item in newThings)
                Entries.Add(item);
        }

        public virtual SystemAnnouncementEntry Set(string content, bool active = false)
        {
            var entry = new SystemAnnouncementEntry(this)
            {
                Message = content,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Active = active
            };
            Add(entry);
            return entry;
        }
        public virtual void Set(string id, SystemAnnouncementEntry entry)
        {
            throw new NotImplementedException();
        }
        public virtual void RemoveId(string id)
        {
            Entries = Entries.Where(v => v.ID != id).ToList();
        }
        public virtual void Add(SystemAnnouncementEntry entry)
        {
            Entries.Add(entry);
            OnUpdate();
        }
        public virtual void Add(string content, bool active = false)
        {
            Add(new SystemAnnouncementEntry(this)
            {
                Message = content,
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Active = active
            });
        }

        public event VoidDelegate Update;
        public event AnnouncementDelegate AnnouncementUpdate;
        public void OnUpdate()
        {
            if (Update != null)
                Update?.Invoke();
        }
        public void OnAnnouncementUpdate(SystemAnnouncementEntry entry)
        {
            if (AnnouncementUpdate != null)
                AnnouncementUpdate.Invoke(entry);
        }

        public virtual SystemAnnouncementSummary GetSummary()
        {
            SystemAnnouncementSummary instance = new SystemAnnouncementSummary()
            {
                Entries = Entries.OrderBy(o => o.Timestamp).ToArray(),
                Active = Active
            };
            return instance;
        }
        public virtual void Read(string content)
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
        public virtual string ToJSON()
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
