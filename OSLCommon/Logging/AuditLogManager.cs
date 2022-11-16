﻿using MongoDB.Driver;
using OSLCommon.Authorization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OSLCommon.Logging
{
    public class AuditLogManager
    {
        public MongoClient mongoClient;
        public string DatabaseName = "opensoftwarelauncher";
        public string CollectionName = "auditLog";

        public AuditLogManager(MongoClient client)
        {
            mongoClient = client;
        }

        public IMongoCollection<AuditLogEntry> GetAuditCollection()
        {
            var db = mongoClient.GetDatabase(DatabaseName);
            var collection = db.GetCollection<AuditLogEntry>(CollectionName);
            return collection;
        }

        public async Task Create(IAuditEntryData entry, Account account)
        {
            var instance = new AuditLogEntry(this);
            instance.ActionData = JsonSerializer.Serialize((object)entry, new JsonSerializerOptions
            {
                IgnoreReadOnlyFields = true,
                IgnoreReadOnlyProperties = true,
                IncludeFields = true
            });
            instance.ActionType = entry.AuditType;
            if (account != null)
                instance.Username = account.Username;

            var collection = GetAuditCollection();
            await collection.InsertOneAsync(instance);
        }

        public async Task<AuditLogEntry[]> GetAll()
        {
            var result = await GetWithFilter(Builders<AuditLogEntry>.Filter.Empty);
            return result;
        }
        public async Task<AuditLogEntry[]> GetByType(AuditType auditType)
        {
            var filter = Builders<AuditLogEntry>
                .Filter
                .Eq("ActionType", (int)auditType);
            var result = await GetWithFilter(filter);
            return result;
        }
        public async Task<AuditLogEntry[]> GetByUsername(string username)
        {
            var filter = Builders<AuditLogEntry>
                .Filter
                .Eq("Username", username);
            var result = await GetWithFilter(filter);
            return result;
        }
        public async Task<AuditLogEntry[]> GetWithFilter(FilterDefinition<AuditLogEntry> filter)
        {
            var collection = GetAuditCollection();
            var found = await collection.FindAsync(filter);
            var arr = found.ToList();
            foreach (var item in arr)
                item.manager = this;
            return arr.ToArray();
        }
    }
}
