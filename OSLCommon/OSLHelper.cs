using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace OSLCommon
{
    public static class OSLHelper
    {
        public static long GetMicroseconds()
        {
            double timestamp = Stopwatch.GetTimestamp();
            double microseconds = 1_000_000f * timestamp / Stopwatch.Frequency;

            return (long)microseconds;
        }

        private static JsonSerializerOptions serializerOptions = null;
        private static void initSerializerOptions()
        {
            serializerOptions = new JsonSerializerOptions
            {
                IgnoreReadOnlyFields = true,
                IgnoreReadOnlyProperties = true,
                IncludeFields = true,
                WriteIndented = true
            };
            serializerOptions.Converters.Add(new kate.shared.DateTimeConverterUsingDateTimeOffsetParse());
            serializerOptions.Converters.Add(new kate.shared.DateTimeConverterUsingDateTimeParse());
        }
        public static JsonSerializerOptions SerializerOptions
        {
            get
            {
                if (serializerOptions == null)
                    initSerializerOptions();
                return serializerOptions;
            }
            set
            {
                serializerOptions = value;
            }
        }
    }
}
