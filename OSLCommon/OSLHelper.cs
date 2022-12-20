using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OSLCommon
{
    public static class OSLHelper
    {
        public static long GetMicroseconds()
        {
            double timestamp = Stopwatch.GetTimestamp();
            double value = 1_000_000f * timestamp / Stopwatch.Frequency;

            return (long)value;
        }
        public static long GetMilliseconds()
        {
            double timestamp = Stopwatch.GetTimestamp();
            double value = 1_000f * timestamp / Stopwatch.Frequency;

            return (long)value;
        }

        public static string[] GetFileList(string directory, string filename)
        {
            var allFiles = new List<string>();
            foreach (var file in Directory.GetFiles(directory))
            {
                if (file.EndsWith(filename))
                    allFiles.Add(file);
            }
            foreach (var dir in Directory.GetDirectories(directory))
            {
                allFiles = new List<string>(allFiles.Concat(GetFileList(dir, filename)));
            }
            return allFiles.ToArray();
        }
        public static List<Type> GetTypesWithAttribute<T>(Assembly assembly)
        {
            List<Type> types = new List<Type>();

            foreach (Type type in assembly.GetTypes())
            {
                if (type.GetCustomAttributes(typeof(T), true).Length > 0)
                    types.Add(type);
            }

            return types;
        }
    }
}
