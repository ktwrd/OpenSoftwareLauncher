using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using OSLCommon.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OSLCommon.Licensing
{
    public class MongoAccountLicenseManager : AccountLicenseManager
    {
        public MongoClient mongoClient;
        public string DatabaseName = "opensoftwarelauncher";
        public string CollectionName = "licenses";
        public string GroupCollectionName = "licenseGroups";
        public MongoAccountLicenseManager(IServiceProvider provider)
            : base(provider.GetService<MongoAccountManager>())
        {
            var accountManager = provider.GetService<MongoAccountManager>();
            var client = provider.GetService<MongoClient>();
            mongoClient = client;
            Update += (field, license) =>
            {
                if (license != null)
                {
                    var collection = GetLicenseCollection<LicenseKeyMetadata>();

                    var filter = Builders<LicenseKeyMetadata>
                        .Filter
                        .Where(v => v.UID == license.UID);

                    long count = collection.Find(filter)?.CountDocuments() ?? 0;
                    if (count < 1)
                        collection.InsertOne(license);
                    else
                        collection.FindOneAndReplace(filter, license);
                }
            };
            LicenseUpdate += (license) =>
            {
                var collection = GetLicenseCollection<LicenseKeyMetadata>();

                var filter = Builders<LicenseKeyMetadata>
                    .Filter
                    .Where(v => v.UID == license.UID);

                long count = collection.Find(filter).CountDocuments();
                if (count < 1)
                    collection.InsertOne(license);
                else
                    collection.FindOneAndReplace(filter, license);
            };
            GroupUpdate += (group) =>
            {
                var collection = GetLicenseCollection<LicenseGroup>();

                var filter = Builders<LicenseGroup>
                .Filter
                    .Where(v => v.UID == group.UID);

                long count = collection.Find(filter)?.CountDocuments() ?? 0;
                if (count < 1)
                    collection.InsertOne(group);
                else
                    collection.FindOneAndReplace(filter, group);
            };
        }
        #region MongoDB Boilerplate
        private IMongoCollection<T> GetLicenseCollection<T>()
        {
            return mongoClient.GetDatabase(DatabaseName).GetCollection<T>(CollectionName);
        }
        private IMongoCollection<T> GetGroupCollection<T>()
        {
            return mongoClient.GetDatabase(DatabaseName).GetCollection<T>(GroupCollectionName);
        }
        #endregion

        public override void RefreshHook()
        {
            Console.WriteLine("[WARN] [MongoAccountLicenseManager] RefreshHook is disabled since hooks are automatically injected when required.");
        }

        public override async Task<LicenseKeyMetadata> GetLicenseKey(string key, bool hook = true)
        {
            var match = LicenseHelper.LicenseKeyRegex.Match(key);
            if (!match.Success) return null;

            var collection = GetLicenseCollection<BsonDocument>();
            var filter = Builders<BsonDocument>
                .Filter
                .Eq("Key", key);
            var result = collection.Find(filter).FirstOrDefault();
            if (result == null)
                return null;

            var deser = BsonSerializer.Deserialize<LicenseKeyMetadata>(result);
            deser.manager = this;
            if (hook)
                HookLicenseEvent(deser);
            return deser;
        }
        public async Task<LicenseKeyMetadata> GetLicenseKeyById(string id, bool hook = true)
        {
            var collection = GetLicenseCollection<BsonDocument>();
            var filter = Builders<BsonDocument>
                .Filter
                .Eq("UID", id);
            var result = collection.Find(filter).FirstOrDefault();
            if (result == null)
                return null;

            var deser = BsonSerializer.Deserialize<LicenseKeyMetadata>(result);
            deser.manager = this;
            if (hook)
                HookLicenseEvent(deser);
            return deser;
        }
        public override async Task<LicenseKeyMetadata[]> GetLicenseKeys(bool hook = true)
        {
            var collection = GetLicenseCollection<LicenseKeyMetadata>();
            var filter = Builders<LicenseKeyMetadata>.Filter.Empty;
            var result = collection.Find(filter).ToList();
            if (hook)
                foreach (var item in result)
                    HookLicenseEvent(item);
            return result.ToArray();
        }
        public override async Task SetLicenseKeys(LicenseKeyMetadata[] keys, bool overwrite = true)
        {
            List<StringOrRegularExpression> keyIds = new List<StringOrRegularExpression>();
            var collection = GetLicenseCollection<LicenseKeyMetadata>();
            int insertCount = 0;
            int replaceCount = 0;
            int deleteCount = 0;
            foreach (var item in keys)
            {
                keyIds.Add(new StringOrRegularExpression(item.UID));

                var filter = Builders<LicenseKeyMetadata>
                    .Filter
                    .Where(v => v.UID == item.UID);

                var found = await collection.FindAsync(filter);
                var count = found.ToList().Count;
                if (count < 1)
                {
                    insertCount++;
                    await collection.InsertOneAsync(item);
                }
                else
                {
                    replaceCount++;
                    await collection.DeleteManyAsync(filter);
                    await collection.InsertOneAsync(item);
                }
                
            }

            if (overwrite)
            {
                var notfilter = Builders<LicenseKeyMetadata>
                    .Filter
                    .Nin("UID", keyIds);
                var items = await collection.FindAsync(notfilter);
                foreach (var i in items.ToList())
                {
                    await DeleteLicenseKey(i.UID);
                    deleteCount++;
                }
            }

            Console.WriteLine($"[MongoAccountLicenseManager->SetLicenseKeys] Insert: {insertCount}, Replace: {replaceCount}, Delete: {deleteCount}");
        }
        public override async Task DeleteLicenseKey(string keyId)
        {
            var licenseCollection = GetLicenseCollection<BsonDocument>();
            var deleteFilter = Builders<BsonDocument>
                .Filter
                .Eq("UID", keyId);
            await licenseCollection.DeleteManyAsync(deleteFilter);

            var groupCollection = GetGroupCollection<LicenseGroup>();
            var groups = groupCollection.Find(Builders<LicenseGroup>.Filter.Empty).ToList();
            foreach (var item in groups)
            {
                if (item.LicenseIds.Contains(keyId))
                {
                    HookLicenseGroupEvent(item);
                    item.LicenseIds = item.LicenseIds.Where(v => v != keyId).ToArray();
                }
            }
        }
        public override async Task<LicenseGroup[]> GetGroups(bool hook = true)
        {
            var filter = GetGroupCollection<LicenseGroup>().Find(Builders<LicenseGroup>.Filter.Empty).ToList();
            if (hook)
                foreach (var item in filter)
                    HookLicenseGroupEvent(item);
            return filter.ToArray();
        }
        public override async Task SetGroups(LicenseGroup[] groups, bool overwrite = true, bool deleteKeys = true)
        {
            var groupIds = new List<StringOrRegularExpression>();
            var groupCollection = GetGroupCollection<LicenseGroup>();
            int insertCount = 0;
            int replaceCount = 0;
            int deleteCount = 0;
            foreach (var item in groups)
            {
                groupIds.Add(new StringOrRegularExpression(item.UID));

                var filter = Builders<LicenseGroup>
                    .Filter
                    .Eq("UID", item.UID);
                var found = await groupCollection.FindAsync(filter);
                var count = found.ToList().Count;
                if (count < 1)
                {
                    insertCount++;
                    await groupCollection.InsertOneAsync(item);
                }
                else
                {
                    replaceCount++;
                    await groupCollection.DeleteManyAsync(filter);
                    await groupCollection.InsertOneAsync(item);
                }
            }

            if (deleteKeys)
            {
                var keyIds = new List<string>();
                var licenseCollection = GetLicenseCollection<LicenseKeyMetadata>();
                var filter = Builders<LicenseKeyMetadata>
                    .Filter
                    .Nin("GroupId", groupIds);
                deleteCount++;
                await licenseCollection.DeleteManyAsync(filter);
            }

            if (overwrite)
            {
                var notFilter = Builders<BsonDocument>
                    .Filter
                    .Nin("UID", groupIds);
                await GetGroupCollection<BsonDocument>().DeleteManyAsync(notFilter);
            }

            Console.WriteLine($"[MongoAccountLicenseManager->SetGroups] Insert: {insertCount}, Replace: {replaceCount}, Delete: {deleteCount}");
        }
        public override async Task DeleteGroup(string groupId, bool includeKeys = true)
        {
            if (includeKeys)
            {
                var licenseCollection = GetLicenseCollection<BsonDocument>();
                var licenseFilter = Builders<BsonDocument>
                    .Filter
                    .Eq("GroupId", groupId);
                await licenseCollection.DeleteManyAsync(licenseFilter);
            }
        }


        #region Enable/Disable
        public override async Task<LicenseKeyActionResult> DisableLicenseKey(string keyId)
        {
            var match = LicenseHelper.LicenseIdRegex.Match(keyId);
            if (!match.Success) return LicenseKeyActionResult.Invalid;

            var license = GetLicenseKeyById(keyId).Result;
            if (license == null)
                return LicenseKeyActionResult.Invalid;
            license.Enable = false;

            var filter = Builders<BsonDocument>
                .Filter
                .Eq("UID", keyId);
            var collection = GetLicenseCollection<BsonDocument>();
            await collection.UpdateManyAsync(filter,
                Builders<BsonDocument>
                .Update
                .Set("Enable", false));

            return LicenseKeyActionResult.Success;
        }
        public override async Task<LicenseKeyActionResult> EnableLicenseKey(string keyId)
        {
            var match = LicenseHelper.LicenseIdRegex.Match(keyId);
            if (!match.Success) return LicenseKeyActionResult.Invalid;

            var license = await GetLicenseKeyById(keyId);
            if (license == null)
                return LicenseKeyActionResult.Invalid;
            license.Enable = true;

            var filter = Builders<BsonDocument>
                .Filter
                .Eq("UID", keyId);
            var collection = GetLicenseCollection<BsonDocument>();
            await collection.UpdateManyAsync(filter,
                Builders<BsonDocument>
                .Update
                .Set("Enable", true));

            return LicenseKeyActionResult.Success;
        }
        #endregion

        public override async Task<CreateLicenseKeyResponse> CreateLicenseKeys(
            string author,
            string[] products,
            int count = 1,
            AccountPermission[] permissions = null,
            string note = "",
            long activateBy = -1,
            string groupLabel = "")
        {
            var createdKeys = createLicenseKeyContent(author, products, count, permissions, note, activateBy, groupLabel);


            var licenseCollection = GetLicenseCollection<LicenseKeyMetadata>();
            foreach (var key in createdKeys.keys)
            {
                HookLicenseEvent(key);
                licenseCollection.InsertOne(key);
            }

            var groupCollection = GetGroupCollection<LicenseGroup>();
            await groupCollection.InsertOneAsync(createdKeys.group);

            OnUpdate(LicenseField.All, null);

            return new CreateLicenseKeyResponse
            {
                Keys = createdKeys.keys,
                GroupId = createdKeys.id
            };
        }
    }
}
