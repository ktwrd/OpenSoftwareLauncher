using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

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
    }
}
