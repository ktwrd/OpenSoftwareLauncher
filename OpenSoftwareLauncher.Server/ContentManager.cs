using OSLCommon.AutoUpdater;
using OSLCommon;
using kate.shared.Helpers;
using System.Text.Json;
using System.Collections.Generic;
using System.IO;
using System;
using System.Diagnostics;
using OSLCommon.Authorization;
using OSLCommon.Licensing;
using System.Linq;
using MongoDB.Driver;
using OSLCommon.Logging;
using OSLCommon.Features;

namespace OpenSoftwareLauncher.Server
{
    public class ContentManager
    {
        public MongoAccountManager AccountManager;
        public MongoSystemAnnouncement SystemAnnouncement;
        public MongoAccountLicenseManager AccountLicenseManager;
        public AuditLogManager AuditLogManager;
        public FeatureManager FeatureManager;

        public MongoClient MongoClient;
        public static string ReleaseInfo_Collection = ServerConfig.GetString("MongoDB", "Collection_ReleaseInfo");
        public static string Publised_Collection = ServerConfig.GetString("MongoDB", "Collection_Published");
        public static string DatabaseName = ServerConfig.GetString("MongoDB", "DatabaseName");

        public ContentManager()
        {
            Log.WriteLine($" Connecting to Database");
            this.MongoClient = new MongoClient(ServerConfig.GetString("Connection", "MongoDBServer"));

            AuditLogManager = new AuditLogManager(MongoClient);
            AuditLogManager.DatabaseName = ServerConfig.GetString("MongoDB", "DatabaseName");
            AuditLogManager.CollectionName = ServerConfig.GetString("MongoDB", "Collection_AuditLog");

            AccountManager = new MongoAccountManager(MongoClient, AuditLogManager);
            AccountManager.DatabaseName = ServerConfig.GetString("MongoDB", "DatabaseName");
            AccountManager.CollectionName = ServerConfig.GetString("MongoDB", "Collection_Account");

            SystemAnnouncement = new MongoSystemAnnouncement(MongoClient);
            SystemAnnouncement.DatabaseName = ServerConfig.GetString("MongoDB", "DatabaseName");
            SystemAnnouncement.CollectionName = ServerConfig.GetString("MongoDB", "Collection_Announcement");

            AccountLicenseManager = new MongoAccountLicenseManager(AccountManager, MongoClient);
            AccountLicenseManager.DatabaseName = ServerConfig.GetString("MongoDB", "DatabaseName");
            AccountLicenseManager.CollectionName = ServerConfig.GetString("MongoDB", "Collection_License");
            AccountLicenseManager.GroupCollectionName = ServerConfig.GetString("MongoDB", "Collection_GroupLicense");

            FeatureManager = new FeatureManager(MongoClient);
            FeatureManager.DatabaseName = ServerConfig.GetString("MongoDB", "DatabaseName");
            FeatureManager.CollectionName = ServerConfig.GetString("MongoDB", "Collection_Features");

            databaseDeserialize();

            SystemAnnouncement.Update += SystemAnnouncement_Update;
        }

        private void SystemAnnouncement_Update()
        {
            ServerConfig.Set("Announcement", "Enable", SystemAnnouncement.Active);
        }

        private readonly string JSONBACKUP_FILENAME = Path.Combine(
            MainClass.DataDirectory,
            "content.json");
        private readonly string JSON_ACCOUNT_FILENAME = Path.Combine(
            MainClass.DataDirectory,
            "account.json");
        private readonly string JSON_SYSANNOUNCE_FILENAME = Path.Combine(
            MainClass.DataDirectory,
            "systemAnnouncement.json");
        private readonly string JSON_LICENSE_FILENAME = Path.Combine(
            MainClass.DataDirectory,
            "licenses.json");
        private readonly string JSON_LICENSEGROUP_FILENAME = Path.Combine(
            MainClass.DataDirectory,
            "licenseGroups.json");
        private class JSONBackupContent
        {
            public List<ReleaseInfo> ReleaseInfoContent = new();
            public Dictionary<string, ProductRelease> Releases = new();
            public Dictionary<string, PublishedRelease> Published = new();
        }
        private void databaseDeserialize()
        {
            if (!ServerConfig.GetBoolean("Migrated", "Account", false))
            {
                try
                {
                    if (File.Exists(JSON_ACCOUNT_FILENAME))
                        AccountManager.ReadJSON(File.ReadAllText(JSON_ACCOUNT_FILENAME));
                }
                catch (Exception except)
                {
                    string txt = $"Failed to read Account Details\n--------\n{except}\n--------\n";
                    Log.Error(txt);
#if DEBUG
                    throw;
#endif
                }
                ServerConfig.Set("Migrated", "Account", true);
            }
            if (!ServerConfig.GetBoolean("Migrated", "Announcement", false))
            {
                try
                {
                    if (File.Exists(JSON_SYSANNOUNCE_FILENAME))
                        SystemAnnouncement.Read(File.ReadAllText(JSON_SYSANNOUNCE_FILENAME));
                    SystemAnnouncement.Active = ServerConfig.GetBoolean("Announcement", "Enable", true);
                }
                catch (Exception except)
                {
                    string txt = $"Failed to read Announcement Details\n--------\n{except}\n--------\n";
                    Log.Error(txt);
#if DEBUG
                    throw;
#endif
                }
                ServerConfig.Set("Migrated", "Announcement", true);
            }
            if (File.Exists(JSONBACKUP_FILENAME))
            {
                RestoreFromJSON();
            }
            if (!ServerConfig.GetBoolean("Migrated", "License", false))
            {
                try
                {
                    if (File.Exists(JSON_LICENSE_FILENAME))
                    {
                        var deser = JsonSerializer.Deserialize<Dictionary<string, LicenseKeyMetadata>>(
                            File.ReadAllText(JSON_LICENSE_FILENAME),
                            MainClass.serializerOptions);
                        AccountLicenseManager.SetLicenseKeys(deser.Select(v => v.Value).ToArray(), false).Wait();
                    }
                }
                catch (Exception except)
                {
                    string txt = $"Failed to read Licenses\n--------\n{except}\n--------\n";
                    Log.Error(txt);
#if DEBUG
                    throw;
#endif
                }
                try
                {
                    if (File.Exists(JSON_LICENSEGROUP_FILENAME))
                    {
                        var deser = JsonSerializer.Deserialize<Dictionary<string, LicenseGroup>>(
                            File.ReadAllText(JSON_LICENSEGROUP_FILENAME),
                            MainClass.serializerOptions);
                        AccountLicenseManager.SetGroups(deser.Select(v => v.Value).ToArray(), false, false).Wait();
                    }
                }
                catch (Exception except)
                {
                    string txt = $"Failed to read License Groups\n--------\n{except}\n--------\n";
                    Log.Error(txt);
#if DEBUG
                    throw;
#endif
                }
                ServerConfig.Set("Migrated", "License", true);
                ServerConfig.Save();
            }
        }
        private void RestoreFromJSON()
        {
            JSONBackupContent? deserialized = null;
            Exception? deserializeException = null;
            try
            {
                deserialized = JsonSerializer.Deserialize<JSONBackupContent>(File.ReadAllText(JSONBACKUP_FILENAME), MainClass.serializerOptions);
            }
            catch
            (Exception e)
            {
                deserializeException = e;
#if DEBUG
                throw;
#endif
            }
            if (deserialized == null)
            {
                string addedContent = "";
                if (deserializeException != null)
                    addedContent = "\n" + deserializeException?.ToString() + "\n";
                Console.Error.Write($"\n[ContentManager->RestoreFromJSON] Failed to restore ;w;{addedContent}");
                return;
            }
            Log.WriteLine($" Restored from JSON.");

            if (!ServerConfig.GetBoolean("Migrated", "ReleaseInfo", false))
            {
                Log.WriteLine($" Importing ReleaseInfo");
                SetReleaseInfoContent(deserialized.ReleaseInfoContent.ToArray());
                ServerConfig.Set("Migrated", "ReleaseInfo", true);
            }
            if (!ServerConfig.GetBoolean("Migrated", "Published", false))
            {
                Log.WriteLine($" Importing Published Releases");
                ForceSetPublishedContent(deserialized.Published.Select(v => v.Value).ToArray());
                ServerConfig.Set("Migrated", "Published", true);
            }
            System.Threading.Thread.Sleep(500);
            DatabaseSerialize();
        }
        public void DatabaseSerialize()
        {
            ServerConfig.Save();
        }
        #region Boilerplate Code
        public void SetReleaseInfoContent(ReleaseInfo[] items)
        {
            var uidList = new List<string>();

            var db = MongoClient.GetDatabase(DatabaseName);
            var collection = db.GetCollection<ReleaseInfo>(ReleaseInfo_Collection);
            foreach (ReleaseInfo item in items)
            {
                var filterCount = Builders<ReleaseInfo>
                    .Filter
                    .Eq("UID", item.UID);
                var count = collection.Find(filterCount).ToList().Count;
                if (count < 1)
                    collection.InsertOne(item);
                else
                    collection.FindOneAndReplace(filterCount, item);
                uidList.Add(item.UID);
            }

            var removeFilter = Builders<ReleaseInfo>
                .Filter
                .Nin("UID", uidList);
            collection.DeleteMany(removeFilter);
        }
        public ReleaseInfo[] GetReleaseInfoContent()
        {
            var db = MongoClient.GetDatabase(DatabaseName);
            var collection = db.GetCollection<ReleaseInfo>(ReleaseInfo_Collection);

            var filter = Builders<ReleaseInfo>
                .Filter
                .Empty;

            var res = collection.Find(filter).ToList();
            return res.ToArray();
        }

        public PublishedRelease? GetPublishedReleaseByHash(string hash)
        {
            var db = MongoClient.GetDatabase(DatabaseName);
            var collection = db.GetCollection<PublishedRelease>(Publised_Collection);

            var filter = Builders<PublishedRelease>
                .Filter
                .Eq("CommitHash", hash);

            return collection.Find(filter).FirstOrDefault();
        }
        public void SetPublishedRelease(PublishedRelease content)
        {
            var db = MongoClient.GetDatabase(DatabaseName);
            var collection = db.GetCollection<PublishedRelease>(Publised_Collection);

            var filter = Builders<PublishedRelease>
                .Filter
                .Eq("UID", content.UID);

            if (collection.Find(filter).ToList().Count < 1)
                collection.InsertOne(content);
            else
                collection.FindOneAndReplace(filter, content);
        }
        public void ForceSetPublishedContent(PublishedRelease[] items)
        {
            var uidList = new List<string>();
            foreach (var i in items)
            {
                uidList.Add(i.UID);
                SetPublishedRelease(i);
            }

            var db = MongoClient.GetDatabase(DatabaseName);
            var collection = db.GetCollection<PublishedRelease>(Publised_Collection);
            var removeFilter = Builders<PublishedRelease>
                .Filter
                .Nin("UID", uidList);
            collection.DeleteMany(removeFilter);
        }
        public PublishedReleaseFile[] GetPublishedFilesByHash(string hash)
        {
            var published = GetPublishedReleaseByHash(hash);
            return published?.Files ?? Array.Empty<PublishedReleaseFile>();
        }
        public void SetPublishedFilesByHash(string hash, PublishedReleaseFile[] files)
        {
            var published = GetPublishedReleaseByHash(hash);
            if (published == null)
                return;
            published.Files = files;
            SetPublishedRelease(published);
        }
        public void AddPublishedFilesByHash(string hash, PublishedReleaseFile[] files)
        {
            var published = GetPublishedReleaseByHash(hash);
            if (published == null)
                return;

            published.Files = published.Files.Concat(files).ToArray();
            SetPublishedRelease(published);
        }
        public Dictionary<string, PublishedRelease> GetAllPublished()
        {
            var db = MongoClient.GetDatabase(DatabaseName);
            var collection = db.GetCollection<PublishedRelease>(Publised_Collection);

            var filter = Builders<PublishedRelease>
                .Filter
                .Empty;

            return collection.Find(filter).ToList().ToDictionary(v => v.CommitHash, t => t);
        }
        public string[] GetAllProductIds()
        {
            var db = MongoClient.GetDatabase(DatabaseName);
            var collection = db.GetCollection<PublishedRelease>(Publised_Collection);

            var filter = Builders<PublishedRelease>
                .Filter
                .Empty;

            return collection.Find(filter).ToList().Where(v => v.Release.appID.Length > 0).Select(v => v.Release.appID).Distinct().ToArray();
        }
        public IMongoCollection<PublishedRelease>? GetPublishedCollection()
        {
            var db = MongoClient.GetDatabase(DatabaseName);
            var collection = db.GetCollection<PublishedRelease>(Publised_Collection);
            return collection;
        }
        public IMongoCollection<ReleaseInfo>? GetReleaseCollection()
        {
            var db = MongoClient.GetDatabase(DatabaseName);
            var collection = db.GetCollection<ReleaseInfo>(ReleaseInfo_Collection);
            return collection;
        }
        #endregion
    }
}
