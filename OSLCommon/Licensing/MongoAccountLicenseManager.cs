using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using OSLCommon.Authorization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OSLCommon.Licensing
{
    public class MongoAccountLicenseManager : AccountLicenseManager
    {
        public MongoClient mongoClient;
        public string DatabaseName = "opensoftwarelauncher";
        public string CollectionName = "licenses";
        public MongoAccountLicenseManager(AccountManager accountManager, MongoClient client)
            : base(accountManager)
        {
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
        }
        private IMongoCollection<T> GetLicenseCollection<T>()
        {
            return mongoClient.GetDatabase(DatabaseName).GetCollection<T>(CollectionName);
        }

        internal void HookLicenseEvent(LicenseKeyMetadata license)
        {
            if (license == null || license.eventHook) return;
            license.eventHook = true;
            Update += (field, eventItem) =>
            {
                if (eventItem != null && eventItem.UID == license.UID)
                {
                    license.Merge(eventItem);
                }
            };
        }

        public override async Task<LicenseKeyMetadata> GetLicenseKey(string key)
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
            HookLicenseEvent(deser);
            return deser;
        }

        public override async Task<LicenseKeyActionResult> DisableLicenseKey(string keyId)
        {
            var match = LicenseHelper.LicenseIdRegex.Match(keyId);
            if (!match.Success) return LicenseKeyActionResult.Invalid;

            var license = await GetLicenseKey(keyId);
            if (license == null)
                return LicenseKeyActionResult.Invalid;
            license.Enable = false;
            return LicenseKeyActionResult.Success;
        }
        public override async Task<LicenseKeyActionResult> EnableLicenseKey(string keyId)
        {
            var match = LicenseHelper.LicenseIdRegex.Match(keyId);
            if (!match.Success) return LicenseKeyActionResult.Invalid;

            var license = await GetLicenseKey(keyId);
            if (license == null)
                return LicenseKeyActionResult.Invalid;
            license.Enable = true;
            return LicenseKeyActionResult.Success;
        }
    }
}
