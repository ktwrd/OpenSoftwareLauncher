using Microsoft.Extensions.DependencyInjection;
using OSLCommon;
using OSLCommon.Authorization;
using OSLCommon.AutoUpdater;
using OSLCommon.Licensing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenSoftwareLauncher.Server
{
    public static class LegacyImport
    {
        private class JSONBackupContent
        {
            public List<ReleaseInfo> ReleaseInfoContent = new();
            public Dictionary<string, ProductRelease> Releases = new();
            public Dictionary<string, PublishedRelease> Published = new();
        }
        public static Task Execute()
        {
            var content = Path.Combine(
                MainClass.DataDirectory,
                "content.json");
            if (File.Exists(content))
                ImportContent(File.ReadAllText(content)).Wait();
            var account = Path.Combine(
                MainClass.DataDirectory,
                "account.json");
            if (File.Exists(account))
                ImportAccount(File.ReadAllText(account)).Wait();
            var announcement = Path.Combine(
                MainClass.DataDirectory,
                "systemAnnouncement.json");
            if (File.Exists(announcement))
                ImportAnnouncement(File.ReadAllText(announcement)).Wait();
            var license = Path.Combine(
                MainClass.DataDirectory,
                "licenses.json");
            var licenseGroups = Path.Combine(
                MainClass.DataDirectory,
                "licenseGroups.json");
            if (File.Exists(license))
                if (File.Exists(licenseGroups))
                    ImportLicense(File.ReadAllText(license), File.ReadAllText(licenseGroups)).Wait();
                else
                    ImportLicense(File.ReadAllText(license), "").Wait();
            else
                if (File.Exists(licenseGroups))
                    ImportLicense("", File.ReadAllText(licenseGroups)).Wait();
            return Task.CompletedTask;
    }
        public static Task ImportContent(string fileContent)
        {
            JSONBackupContent? deserialized = null;
            try
            {
                deserialized = JsonSerializer.Deserialize<JSONBackupContent>(fileContent, MainClass.serializerOptions);

            }
            catch
            {
#if DEBUG
                throw;
#endif
            }

            if (deserialized == null)
                return Task.CompletedTask;

            var mongoMiddle = MainClass.Provider?.GetService<MongoMiddle>();
            
            if (!MainClass.Config.Migrated.ReleaseInfo)
            {
                mongoMiddle?.SetReleaseInfoContent(deserialized.ReleaseInfoContent.ToArray());
                MainClass.Config.Migrated.ReleaseInfo = true;
            }
            if (!MainClass.Config.Migrated.Published)
            {
                mongoMiddle?.ForceSetPublishedContent(deserialized.Published.Select(v => v.Value).ToArray());
                MainClass.Config.Migrated.Published = true;
            }
            MainClass.Save();
            return Task.CompletedTask;
        }

        public static Task ImportAccount(string fileContent)
        {
            if (MainClass.Config.Migrated.Account)
                return Task.CompletedTask;

            MainClass.Provider?.GetService<MongoAccountManager>()?.ReadJSON(fileContent);
            MainClass.Config.Migrated.Account = true;
            MainClass.Save();

            return Task.CompletedTask;
        }

        public static Task ImportAnnouncement(string fileContent)
        {
            if (MainClass.Config.Migrated.Announcement)
                return Task.CompletedTask;

            MainClass.Provider?.GetService<MongoSystemAnnouncement>()?.Read(fileContent);
            MainClass.Config.Migrated.Announcement = true;
            MainClass.Save();
            return Task.CompletedTask;
        }

        public static Task ImportLicense(string licenseFileContent, string groupFileContent)
        {
            if (MainClass.Config.Migrated.License)
                return Task.CompletedTask;
            if (licenseFileContent.Length > 1)
            {
                var licenseContent = (JsonSerializer.Deserialize<Dictionary<string, LicenseKeyMetadata>>(licenseFileContent.ToString(), MainClass.serializerOptions) ?? new Dictionary<string, LicenseKeyMetadata>())
                        .Select(v => v.Value).ToArray();

                MainClass.Provider?.GetService<MongoAccountLicenseManager>()?.SetLicenseKeys(licenseContent, false).Wait();
            }
            if (groupFileContent.Length > 1)
            {
                var groupContent = (JsonSerializer.Deserialize<Dictionary<string, LicenseGroup>>(groupFileContent, MainClass.serializerOptions) ?? new Dictionary<string, LicenseGroup>())
                    .Select(v => v.Value).ToArray();
                MainClass.Provider?.GetService<MongoAccountLicenseManager>()?.SetGroups(groupContent, false);
            }
            MainClass.Config.Migrated.License = true;
            MainClass.Save();
            return Task.CompletedTask;
        }
    }
}
