using kate.shared.Helpers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace OSLCommon.Logging
{
    public interface IAuditEntryData
    {
        AuditType AuditType { get; set; }

        string SerializeToJSON();
    }
    public class AuditLogEntry
    {
        [JsonIgnore]
        [BsonIgnore]
        [XmlIgnore]
        [SoapIgnore]
        internal AuditLogManager manager = null;
        public AuditLogEntry()
        {
            UID = GeneralHelper.GenerateUID();
            Username = "";
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            ActionType = AuditType.None;
            ActionData = "";
        }
        public AuditLogEntry(AuditLogManager manager)
            : this()
        {
            this.manager = manager;
        }
        [JsonIgnore]
        [XmlIgnore]
        [SoapIgnore]
        public ObjectId _id { get; set; }
        public string UID { get; set; }
        public string Username { get; set; }
        public long Timestamp { get; set; }
        public AuditType ActionType { get; set; }
        public string ActionData { get; set; }

        public T DeserializeData<T>() where T : IAuditEntryData
        {
            var result = JsonSerializer.Deserialize<T>(ActionData, new JsonSerializerOptions
            {
                IgnoreReadOnlyFields = true,
                IgnoreReadOnlyProperties = true,
                IncludeFields = true
            });
            result.AuditType = ActionType;
            return result;
        }

        public string SerializeData()
        {
            return ActionData;
        }
    }
}
