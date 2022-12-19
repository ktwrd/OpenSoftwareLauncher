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

            SystemAnnouncement.Update += SystemAnnouncement_Update;
        }

        private void SystemAnnouncement_Update()
        {
            ServerConfig.Set("Announcement", "Enable", SystemAnnouncement.Active);
        }
        public void DatabaseSerialize()
        {
            ServerConfig.Save();
        }
        
    }
}
