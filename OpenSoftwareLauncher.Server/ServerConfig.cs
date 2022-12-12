using Amazon.Runtime.Internal.Transform;
using Google.Api.Gax.Grpc.Gcp;
using Google.Cloud.Firestore.V1;
using kate.shared.Helpers;
using Nini.Config;
using OSLCommon.AutoUpdater;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace OpenSoftwareLauncher.Server
{
    public static class ServerConfig
    {
        public static string ConfigFilename => "config.ini";
        public static string ConfigLocation
        => Path.Combine(
            MainClass.DataDirectory,
            "Config",
            ConfigFilename);
        public static string OldConfigLocation
        => Path.Combine(
                MainClass.DataDirectory,
                ConfigFilename);
        public static IniConfigSource Source;
        private static Timer BusStationTimer;
        static ServerConfig()
        {
            if (!Directory.Exists(Path.GetDirectoryName(ConfigLocation)))
                Directory.CreateDirectory(Path.GetDirectoryName(ConfigLocation));
            if (File.Exists(OldConfigLocation) && !File.Exists(ConfigLocation))
            {
                File.Move(OldConfigLocation, ConfigLocation);
                Console.WriteLine($"[ServerConfig] Moved config to {ConfigLocation}");
            }

            if (!File.Exists(ConfigLocation))
                File.WriteAllText(ConfigLocation, "");


            ReadFrom(ConfigLocation);
        }
        internal static void ReadFrom(string? location=null)
        {
            var resetEvent = new AutoResetEvent(false);
            if (BusStationTimer != null)
            {
                BusStationTimer = new Timer(delegate
                {
                    if (HasChanges)
                    {
                        Save();
                        HasChanges = false;
                    }
                    resetEvent.Set();
                }, resetEvent, 0, 1000);
            }
            Source = new IniConfigSource(location ?? ConfigLocation);
            MergeDefaultData();
            if (BusStationTimer != null)
                resetEvent.WaitOne(1);
        }

        private static void MergeDefaultData()
        {
            foreach (var groupPair in DefaultData)
            {
                foreach (var pair in groupPair.Value)
                {
                    if (!Get(groupPair.Key).Contains(pair.Key))
                    {
                        Set(groupPair.Key, pair.Key, pair.Value);
                    }
                }
            }
            Save();
        }

        public static Dictionary<string, Dictionary<string, object>> DefaultData = new Dictionary<string, Dictionary<string, object>>()
        {
            {"Security", new Dictionary<string, object>()
                {
                    {"AllowAdminOverride", true },
                    {"AllowPermission_ReadReleaseBypass", true },
                    {"AllowGroupRestriction", false },
                    {"RequireAuthentication", true },
                    {"DefaultSignatures", "" },
                    {"ImmuneUsers", "" }
                }
            },
            {"Authentication", new Dictionary<string, object>()
                {
                    {"Provider", "" },
                    {"ProviderSignupURL", "" }
                }
            },
            {"Telemetry", new Dictionary<string, object>()
                {
                    {"Prometheus", false }
                }
            },
            {"Connection", new Dictionary<string, object>()
                {
                    {"MongoDBServer", "" }
                }
            },
            {"MongoDB", new Dictionary<string, object>
                {
                    {"DatabaseName", "opensoftwarelauncher" },
                    {"Collection_Account", "accounts" },
                    {"Collection_Announcement", "announcements" },
                    {"Collection_License", "licenses" },
                    {"Collection_GroupLicense", "licenseGroups" },
                    {"Collection_ReleaseInfo", "releaseInfo" },
                    {"Collection_Published", "published" },
                    {"Collection_AuditLog", "auditLog" },
                    {"Collection_Features", "features" }
                }
            },
            {"Migrated", new Dictionary<string, object>
                {
                    {"Account", false },
                    {"Announcement", false },
                    {"License", false },
                    {"ReleaseInfo", false },
                    {"Published", false }
                }
            }
        };

        public static string[] Security_DefaultSignatures
        {
            get
            {
                return GetString("Security", "DefaultSignatures", "").Split(' ');
            }
            set
            {
                Set("Security", "DefaultSignatures", string.Join(' ', value));
            }
        }
        /// <summary>
        /// Immune users are users who cannot be banned.
        /// </summary>
        public static string[] Security_ImmuneUsers
        {
            get
            {
                return GetString("Security", "ImmuneUsers", "").Split(',');
            }
            set
            {
                Set("Security", "ImmuneUsers", string.Join(',', value));
            }
        }

        public delegate void ConfigSetDelegate(string group, string key, object value);
        public static ConfigSetDelegate OnWrite;
        public static VoidDelegate OnSave;

        public static void Save()
        {
            var startNS = GeneralHelper.GetNanoseconds();
            if (!File.Exists(ConfigLocation))
                File.WriteAllText(ConfigLocation, "");
            Source.Save();
            Console.WriteLine($"[ServerConfig] Saved {GeneralHelper.GetNanoseconds() - startNS}ns");
            HasChanges = false;
            OnSave?.Invoke();
        }

        public static Dictionary<string, Dictionary<string, object>> Get()
        {
            var dict = new Dictionary<string, Dictionary<string, object>>();
            foreach (IConfig cfg in Source.Configs)
            {
                if (cfg == null) continue;
                dict.Add(cfg.Name, new Dictionary<string, object>());
                foreach (var key in cfg.GetKeys())
                {
                    var value = cfg.Get(key);
                    dict[cfg.Name].Add(key, value);
                }
            }
            return dict;
        }
        public static void Set(Dictionary<string, Dictionary<string, object>> dict)
        {
            foreach (var group in dict)
            {
                foreach (var item in group.Value)
                {
                    Set(group.Key, item.Key, item.Value);
                }
            }
            HasChanges = true;
            Save();
        }

        public static IConfig Get(string group)
        {
            var cfg = Source.Configs[group];
            if (cfg == null)
                cfg = Source.Configs.Add(group);
            return cfg;
        }
        public static void Set(string group, string key, object value)
        {
            var cfg = Get(group);
            cfg.Set(key, value);
            HasChanges = true;
        }

        private static bool HasChanges = false;

        public static string Get(string group, string key) => Get(group).Get(key);
        public static string Get(string group, string key, string fallback) => Get(group).Get(key, fallback);

        public static string GetExpanded(string group, string key) => Get(group).GetExpanded(key);

        public static string GetString(string group, string key) => Get(group).GetString(key);
        public static string GetString(string group, string key, string fallback) => Get(group).GetString(key, fallback);

        public static int GetInt(string group, string key) => Get(group).GetInt(key);
        public static int GetInt(string group, string key, int fallback) => Get(group).GetInt(key, fallback);
        public static int GetInt(string group, string key, int fallback, bool fromAlias) => Get(group).GetInt(key, fallback, fromAlias);

        public static long GetLong(string group, string key) => Get(group).GetLong(key);
        public static long GetLong(string group, string key, long fallback) => Get(group).GetLong(key, fallback);

        public static bool GetBoolean(string group, string key) => Get(group).GetBoolean(key);
        public static bool GetBoolean(string group, string key, bool fallback) => Get(group).GetBoolean(key, fallback);

        public static float GetFloat(string group, string key) => Get(group).GetFloat(key);
        public static float GetFloat(string group, string key, float fallback) => Get(group).GetFloat(key, fallback);

        public static string[] GetKeys(string group) => Get(group).GetKeys();
        public static string[] GetValues(string group) => Get(group).GetValues();
        public static void Remove(string group, string key) => Get(group).Remove(key);
    }
}
