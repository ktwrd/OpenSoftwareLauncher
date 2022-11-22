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

        public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

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
