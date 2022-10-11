using Google.Api.Gax.Grpc.Gcp;
using Nini.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenSoftwareLauncher.Server
{

    namespace OpenSoftwareLauncher.Server
    {
        public static class ServerConfig
        {
            public static string ConfigFilename => "config.ini";
            public static string ConfigLocation => Path.Combine(MainClass.DataDirectory, ConfigFilename);
            public static IniConfigSource Source;
            private static Timer BusStationTimer;
            static ServerConfig()
            {
                var resetEvent = new AutoResetEvent(false);
                BusStationTimer = new Timer(delegate
                {
                    if (HasChanges)
                    {
                        Save();
                        HasChanges = false;
                    }
                    resetEvent.Set();
                }, resetEvent, 0, 1000);
                if (!File.Exists(ConfigLocation))
                {
                    File.WriteAllText(ConfigLocation, "");
                }
                Source = new IniConfigSource(ConfigLocation);
                MergeDefaultData();
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
                        {"RequireAuthentication", true }
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
                }
            };

            public static void Save()
            {
                var startTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                if (!File.Exists(ConfigLocation))
                    File.WriteAllText(ConfigLocation, "");
                Source.Save();
                Console.WriteLine($"[ServerConfig] Saved {DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - startTimestamp}");
                HasChanges = false;
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

}
