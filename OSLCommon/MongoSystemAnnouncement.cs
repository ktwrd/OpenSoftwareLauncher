using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OSLCommon
{
    public class MongoSystemAnnouncement : SystemAnnouncement
    {
        internal MongoClient mongoClient;
        public MongoSystemAnnouncement(MongoClient client)
        {
            mongoClient = client;
            AnnouncementUpdate += (entry) =>
            {
                var collection = GetAnnouncementCollection<SystemAnnouncementEntry>();
                var filter = Builders<SystemAnnouncementEntry>.Filter.Eq("ID", entry.ID);
                collection.ReplaceOne(filter, entry);
            };
        }
        public string DatabaseName = "opensoftwarelauncher";
        public string CollectionName = "announcements";
        private IMongoCollection<T> GetAnnouncementCollection<T>()
        {
            return mongoClient.GetDatabase(DatabaseName).GetCollection<T>(CollectionName);
        }

        public override SystemAnnouncementEntry GetLatest()
        {
            var filter = Builders<SystemAnnouncementEntry>
                .Filter
                .Eq("Active", true);

            var exec = GetAnnouncementCollection<SystemAnnouncementEntry>()
                .Find(filter)
                .ToEnumerable()
                .OrderBy(o => o.Timestamp)
                .ToList();

            SystemAnnouncementEntry target = null;
            long ts = 0;
            foreach (var item in exec)
            {
                if (item.Timestamp > ts)
                    target = item;
            }
            InjectEntry(target);
            return target;
        }
        public override SystemAnnouncementEntry[] GetAll()
        {
            var filter = Builders<SystemAnnouncementEntry>
                .Filter
                .Where(v => v.Timestamp > 0);
            return GetAnnouncementCollection<SystemAnnouncementEntry>()
                    .Find(filter)
                    .ToList()
                    .ToArray();
        }
        public override void Set(SystemAnnouncementEntry[] all)
        {
            var collection = GetAnnouncementCollection<SystemAnnouncementEntry>();
            foreach (var item in all)
            {
                var filter = Builders<SystemAnnouncementEntry>
                    .Filter
                    .Eq("ID", item.ID);
                var exist = collection.Find(filter).ToList();
                if (exist.Count < 1)
                    collection.InsertOne(item);
                else if (exist.Count > 0)
                    collection.FindOneAndReplace(filter, item);
            }
        }
        public override void Add(SystemAnnouncementEntry entry)
        {
            var collection = GetAnnouncementCollection<SystemAnnouncementEntry>();
            var filter = Builders<SystemAnnouncementEntry>
                .Filter
                .Eq("ID", entry.ID);
            var exist = collection.Find(filter).ToList().Count > 0;
            if (exist)
                collection.FindOneAndReplace(filter, entry);
            else
                collection.InsertOne(entry);
        }
    }
}
