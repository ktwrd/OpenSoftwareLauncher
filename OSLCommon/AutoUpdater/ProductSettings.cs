using kate.shared.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace OSLCommon.AutoUpdater
{
    public class ProductSettings
    {
        public bool AutoLaunch = false;
        public bool AutoUpdate = false;
        public string LaunchArguments = @"";
        public int MemoryLimit = 1024;
        public bool HardwareAcceleration = true;

        public void Load(string location)
        {
            if (!File.Exists(location))
                Write(location);
            var dict = new Dictionary<string, string>()
            {
                { @"AutoUpdate", @"0" },
                { @"AutoLaunch", @"0" },
                { @"LaunchArguments", @"" },
                { @"MemoryLimit", @"1024" },
                { @"HardwareAcceleration", @"1" }
            };
            var lines = File.ReadAllLines(location);
            foreach (var line in lines)
            {
                var key = line.Split(new string[] { "=" }, StringSplitOptions.None)[0];
                var value = line.Replace($"{key}=", "");
                if (dict.ContainsKey(key))
                    dict[key] = value;
                else
                    dict.Add(key, value);
            }

            LaunchArguments = dict["LaunchArguments"];
            AutoUpdate = Convert.ToInt32(dict["AutoUpdate"]) == 1;
            HardwareAcceleration = Convert.ToInt16(dict["HardwareAcceleration"]) == 1;
            LaunchArguments = dict["LaunchArguments"];
            MemoryLimit = Convert.ToInt32(dict["MemoryLimit"]);
        }

        public void Write(string location)
        {
            var dict = new Dictionary<string, string>()
            {
                { @"AutoUpdate", AutoUpdate ? "1" : "0" },
                { @"AutoLaunch", AutoLaunch ? "1" : "0" },
                { @"LaunchArguments", LaunchArguments },
                { @"MemoryLimit", MemoryLimit.ToString() },
                { @"HardwareAcceleration", HardwareAcceleration ? "1" : "0" }
            };
            var lines = new List<string>();
            foreach (var pair in dict)
            {
                lines.Add($"{pair.Key}={pair.Value}");
            }
            File.WriteAllLines(location, lines.ToArray());
        }
    }
}
