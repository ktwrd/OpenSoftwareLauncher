﻿using OSLCommon.AutoUpdater;
using OSLCommon;
using Google.Cloud.Firestore;
using kate.shared.Helpers;
using System.Text.Json;
using System.Collections.Generic;
using System.IO;
using System;
using System.Diagnostics;
using OSLCommon.Authorization;
using OpenSoftwareLauncher.Server.OpenSoftwareLauncher.Server;
using OSLCommon.Licensing;
using System.Linq;
using MongoDB.Driver;

namespace OpenSoftwareLauncher.Server
{
    public class ContentManager
    {
        public List<ReleaseInfo> ReleaseInfoContent = new();
        public Dictionary<string, ProductRelease> Releases = new();
        public Dictionary<string, PublishedRelease> Published = new();
        public MongoAccountManager AccountManager;
        public MongoSystemAnnouncement SystemAnnouncement;
        public AccountLicenseManager AccountLicenseManager;

        public MongoClient MongoClient;

        public ContentManager()
        {
            this.MongoClient = new MongoClient(ServerConfig.GetString("Connection", "MongoDBServer"));

            AccountManager = new MongoAccountManager(MongoClient);
            SystemAnnouncement = new MongoSystemAnnouncement(MongoClient);

            AccountLicenseManager = new AccountLicenseManager(AccountManager);
            databaseDeserialize();

            AccountManager.PendingWrite += AccountManager_PendingWrite;
            SystemAnnouncement.Update += SystemAnnouncement_Update;
            AccountLicenseManager.Update += AccountLicenseManager_Update;
        }

        private void AccountLicenseManager_Update(LicenseField field, LicenseKeyMetadata license)
        {
            if (field == LicenseField.All || field == LicenseField.AllGroups)
            {
                File.WriteAllText(JSON_LICENSEGROUP_FILENAME,
                    JsonSerializer.Serialize(AccountLicenseManager.LicenseGroups, MainClass.serializerOptions));
            }
            if (field == LicenseField.AllGroups)
            {
                File.WriteAllText(JSON_LICENSE_FILENAME,
                    JsonSerializer.Serialize(AccountLicenseManager.LicenseKeys, MainClass.serializerOptions));
            }
        }

        private void SystemAnnouncement_Update()
        {
            ServerConfig.Set("Announcement", "Enable", SystemAnnouncement.Active);
            return;
            File.WriteAllText(JSON_SYSANNOUNCE_FILENAME, SystemAnnouncement.ToJSON());
            string txt = $"[ContentManager->SystemAnnouncement_Update]  {Path.GetRelativePath(Directory.GetCurrentDirectory(), JSON_SYSANNOUNCE_FILENAME)}";
            Trace.WriteLine(txt);
            Console.WriteLine(txt);
            ServerConfig.Save();
        }

        private void AccountManager_PendingWrite()
        {
            return;
            File.WriteAllText(JSON_ACCOUNT_FILENAME, AccountManager.ToJSON());
            AccountManager.ClearPendingWrite();
            string txt = $"[ContentManager->AccountManager_PendingWrite] {Path.GetRelativePath(Directory.GetCurrentDirectory(), JSON_ACCOUNT_FILENAME)}";
            Trace.WriteLine(txt);
            Console.WriteLine(txt);
            ServerConfig.Save();
        }

        /*private readonly string DATABASE_FILENAME = Path.Combine(
            MainClass.DataDirectory,
            "content.db");*/
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
                    string txt = $"[ContentManager->databaseSerialize:{GeneralHelper.GetNanoseconds()}] [ERR] Failed to read Account Details\n--------\n{except}\n--------\n";
                    Trace.WriteLine(txt);
                    Console.Error.WriteLine(txt);
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
                    string txt = $"[ContentManager->databaseSerialize:{GeneralHelper.GetNanoseconds()}] [ERR] Failed to read Announcement Details\n--------\n{except}\n--------\n";
                    Trace.WriteLine(txt);
                    Console.Error.WriteLine(txt);
                }
                ServerConfig.Set("Migrated", "Announcement", true);
            }
            if (File.Exists(JSONBACKUP_FILENAME))
            {
                RestoreFromJSON();
            }
            try
            {
                if (File.Exists(JSON_LICENSE_FILENAME))
                    AccountLicenseManager.LicenseKeys = JsonSerializer.Deserialize<Dictionary<string, LicenseKeyMetadata>>(
                        File.ReadAllText(JSON_LICENSE_FILENAME),
                        MainClass.serializerOptions);
            }
            catch (Exception except)
            {
                string txt = $"[ContentManager->databaseSerialize:{GeneralHelper.GetNanoseconds()}] [ERR] Failed to read Licenses\n--------\n{except}\n--------\n";
                Trace.WriteLine(txt);
                Console.Error.WriteLine(txt);
            }
            try
            {
                if (File.Exists(JSON_LICENSEGROUP_FILENAME))
                    AccountLicenseManager.LicenseGroups = JsonSerializer.Deserialize<Dictionary<string, LicenseGroup>>(
                        File.ReadAllText(JSON_LICENSEGROUP_FILENAME),
                        MainClass.serializerOptions);
            }
            catch (Exception except)
            {
                string txt = $"[ContentManager->databaseSerialize:{GeneralHelper.GetNanoseconds()}] [ERR] Failed to read License Groups\n--------\n{except}\n--------\n";
                Trace.WriteLine(txt);
                Console.Error.WriteLine(txt);
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
            }
            if (deserialized == null)
            {
                string addedContent = "";
                if (deserializeException != null)
                    addedContent = "\n" + deserializeException?.ToString() + "\n";
                Console.Error.Write($"\n[ContentManager->RestoreFromJSON] Failed to restore ;w;{addedContent}");
                return;
            }
            Console.WriteLine($"[ContentManager->RestoreFromJSON] Restored from JSON.");

            ReleaseInfoContent = deserialized.ReleaseInfoContent;
            Releases = ReleaseHelper.TransformReleaseList(ReleaseInfoContent.ToArray());
            Published = deserialized.Published;
            System.Threading.Thread.Sleep(500);
            DatabaseSerialize();
        }
        public void DatabaseSerialize()
        {
            SystemAnnouncement.OnUpdate();
            AccountManager.ForcePendingWrite();
            AccountLicenseManager.OnUpdate(LicenseField.All, null);
            CreateJSONBackup();
            ServerConfig.Save();
        }
        private void CreateJSONBackup()
        {
            var data = new JSONBackupContent
            {
                ReleaseInfoContent = ReleaseInfoContent.ToArray().ToList(),
                Releases = Releases,
                Published = Published
            };
            File.WriteAllText(JSONBACKUP_FILENAME, JsonSerializer.Serialize(data, MainClass.serializerOptions));
        }
    }
}
