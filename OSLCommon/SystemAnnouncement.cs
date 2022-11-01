using kate.shared.Helpers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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


    public class SystemAnnouncementEntry
    {
        [BsonIgnore]
        [JsonIgnore]
        internal SystemAnnouncement manager;
        [BsonIgnore]
        [JsonIgnore]
        internal bool hooked = false;
        [JsonIgnore]
        public ObjectId _id { get; set; }
        public string Message 
        {
            get => message;
            set
            {
                message = value;
                if (manager != null)
                    manager.OnAnnouncementUpdate(this);
            }
        }
        internal string message;
        public long Timestamp
        {
            get => timestamp;
            set
            {
                timestamp = value;
                if (manager != null)
                    manager.OnAnnouncementUpdate(this);
            }
        }
        internal long timestamp;
        public bool Active
        {
            get => active;
            set
            {
                active = value;
                if (manager != null)
                    manager.OnAnnouncementUpdate(this);
            }
        }
        internal bool active;
        public string ID { get; set; }
        public SystemAnnouncementEntry(SystemAnnouncement manager)
        {
            this.manager = manager;
            ID = GeneralHelper.GenerateToken(12);
            Message = "";
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            Active = false;
        }
        public SystemAnnouncementEntry() : this(null)
        { }
        public void Merge(SystemAnnouncementEntry entry)
        {
            active = entry.active;
            timestamp = entry.timestamp;
            message = entry.message;
        }
    }
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
