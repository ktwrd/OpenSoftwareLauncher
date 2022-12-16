using kate.shared.Helpers;
using Nini.Config;
using OSLCommon;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenSoftwareLauncher.DesktopWinForms
{
    public static class UserConfig
    {
        public static string ConfigFilename => "config.ini";
        public static string ConfigLocation => Path.Combine(Path.GetDirectoryName(Application.ExecutablePath) ?? Directory.GetCurrentDirectory(), ConfigFilename);
        public static IniConfigSource Source;
        static UserConfig()
        {
            if (!File.Exists(ConfigLocation))
                File.WriteAllText(ConfigLocation, "");
            Source = new IniConfigSource(ConfigLocation);
            Save();
        }
        public static bool PendingWrite { get; private set; }
        public static event ComparisonDelegate<bool> PendingWriteChanged;
        public static event VoidDelegate SaveEvent;

        #region Config Shortcuts
        public static string Auth_Username
        {
            get { return GetString("Authentication", "Username", ""); }
            set { Set("Authentication", "Username", value); }
        }
        public static string Auth_Token
        {
            get { return GetString("Authentication", "Token", ""); }
            set { Set("Authentication", "Token", value); }
        }
        public static bool Auth_Remember
        {
            get { return GetBoolean("Authentication", "Remember", false); }
            set { Set("Authentication", "Remember", value); }
        }

        public static string Connection_Endpoint
        {
            get { return GetString("Connection", "Endpoint", ""); }
            set { Set("Connection", "Endpoint", value); }
        }

        public static long AuditLog_DefaultTimeRangeMinOffset
        {
            get
            {
                return long.Parse(GetString("AuditLog", "DefaultTimeRange_MinOffset", "-86400000"));
            }
            set
            {
                Set("AuditLog", "DefaultTimeRange_MinOffset", value.ToString());
            }
        }
        #endregion


        public static Dictionary<string, Dictionary<string, object>> DefaultData = new Dictionary<string, Dictionary<string, object>>()
        {
            {"Authentication", new Dictionary<string, object>()
                {
                    {"Username", "" },
                    {"Token", "" },
                    {"Remember", false }
                }
            },
            {"Connection", new Dictionary<string, object>()
                {
                    {"Endpoint", "" }
                }
            },
            {"General", new Dictionary<string, object>()
                {
                    {"Language", "en" },
                    {"ShowLatestRelease", false }
                }
            },
            {"AuditLog", new Dictionary<string, object>()
                {
                    {"DefaultTimeRange_MinOffset", -86400000 }
                }
            }
        };

        public static void Save()
        {
            var start = OSLHelper.GetMicroseconds();
            if (!File.Exists(ConfigLocation))
                File.WriteAllText(ConfigLocation, "");
            MergeDefaultData();
            Source.Save();
            PendingWriteChanged?.Invoke(false, PendingWrite);
            PendingWrite = false;
            SaveEvent?.Invoke();
            Trace.WriteLine($"[UserConfig] Saved {OSLHelper.GetMicroseconds() - start}µs");
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
            Source.Save();
        }

        public static Dictionary<string, Dictionary<string, string>> Get()
        {
            var dict = new Dictionary<string, Dictionary<string, string>>();
            foreach (IConfig cfg in Source.Configs)
            {
                if (cfg == null) continue;
                dict.Add(cfg.Name, new Dictionary<string, string>());
                foreach (var key in cfg.GetKeys())
                {
                    var value = cfg.Get(key);
                    dict[cfg.Name].Add(key, value);
                }
            }
            return dict;
        }
        public static void Set(Dictionary<string, Dictionary<string, string>> dict)
        {
            foreach (var group in dict)
            {
                foreach (var item in group.Value)
                {
                    Set(group.Key, item.Key, item.Value);
                }
            }
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
            PendingWriteChanged?.Invoke(true, PendingWrite);
            PendingWrite = true;
        }

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
