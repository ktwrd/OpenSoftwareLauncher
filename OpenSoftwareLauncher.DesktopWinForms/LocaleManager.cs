﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenSoftwareLauncher.DesktopWinForms
{
    public static class LocaleManager
    {
        public static Dictionary<string, Dictionary<string, string>> LanguageDatabase = new Dictionary<string, Dictionary<string, string>>();

        public const string FallbackLanguage = "en";
        public static string Language = "en";

        public static string LocaleDirectory
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Locale");
            }
        }
        static LocaleManager()
        {
            Load();
        }
        public static void Load()
        {
            var files = Directory.GetFiles(LocaleDirectory);
            LanguageDatabase.Clear();
            Trace.WriteLine(@"[LocaleManager] Loading Locale");
            foreach (var location in files)
            {
                var filename = Path.GetFileNameWithoutExtension(location);
                var langName = filename.Split('-')[0];
                if (!LanguageDatabase.ContainsKey(langName))
                    LanguageDatabase.Add(langName, new Dictionary<string, string>());
                var count = 0;
                foreach (var fileLine in File.ReadLines(location))
                {
                    var key = fileLine.Split('=')[0];
                    if (key.Length < 1) continue;
                    var value = fileLine.Substring(fileLine.IndexOf('=') + 1);
                    if (!LanguageDatabase[langName].ContainsKey(key))
                        LanguageDatabase[langName].Add(key, value);
                    LanguageDatabase[langName][key] = value;
                    count++;
                }
                Trace.WriteLine($"[LocaleManager] {Path.GetFileName(location)} has {count} entries");
            }
        }

        public static string Get(string key, string lang="", string fallback=null, Dictionary<string, object> inject = null)
        {
            var targetLang = LanguageDatabase.ContainsKey(lang) ? lang : FallbackLanguage;
            if (fallback == null)
                fallback = key;

            var value = fallback;
            if (LanguageDatabase[targetLang].ContainsKey(key))
            {
                value = LanguageDatabase[targetLang][key];
                Trace.WriteLine($"[LocaleManager->Get] Failed to find item \"{targetLang}.{key}\"");
            }
            else if (LanguageDatabase[FallbackLanguage].ContainsKey(key))
            {
                value = LanguageDatabase[FallbackLanguage][key];
                Trace.WriteLine($"[LocaleManager->Get] Failed to find item \"{key}\" in fallback language. Returning fallback string");
            }

            var dictReplace = new Dictionary<string, object>()
            {
                { @"version", Program.SoftwareVersion },
                { @"product.name", Program.ProductName },
                { @"product.name.short", Program.ProductNameAcronym },
                { @"nl", "\n" },
                { @"newline", "\n" }
            };

            if (inject != null)
            foreach (var item in inject)
            {
                if (!dictReplace.ContainsKey(item.Key))
                    dictReplace.Add(item.Key, item.Value);
                else
                    dictReplace[item.Key] = item.Value;
            }

            foreach (var pair in dictReplace)
            {
                value = value.Replace("${" + pair.Key + "}", pair.Value.ToString());
            }

            return value;
        }
    }
}
