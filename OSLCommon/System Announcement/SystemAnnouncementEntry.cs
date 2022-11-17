using kate.shared.Helpers;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.Json.Serialization;

namespace OSLCommon
{
    public class SystemAnnouncementEntry
    {
        [BsonIgnore]
        [JsonIgnore]
        [Browsable(false)]
        internal SystemAnnouncement manager;
        [BsonIgnore]
        [JsonIgnore]
        [Browsable(false)]
        internal bool hooked = false;
        [JsonIgnore]
        [Browsable(false)]
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
}
