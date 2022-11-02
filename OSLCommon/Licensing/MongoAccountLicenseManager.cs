﻿using MongoDB.Bson;
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
        public string GroupCollectionName = "licenseGroups";
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
        public virtual async Task<LicenseKeyMetadata[]> GetLicenseKeys(bool hook = true)
        {
            var collection = GetLicenseCollection<LicenseKeyMetadata>();
            var filter = Builders<LicenseKeyMetadata>.Filter.Empty;
            var result = collection.Find(filter).ToList();
            if (hook)
                foreach (var item in result)
                    HookLicenseEvent(item);
            return result.ToArray();
        }
        public virtual async void SetLicenseKeys(LicenseKeyMetadata[] keys, bool overwrite = true)
        {
            List<string> keyIds = new List<string>();
        }

        #region Enable/Disable
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
