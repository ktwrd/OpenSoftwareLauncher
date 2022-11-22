using kate.shared.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace OSLCommon.Logging.Elastic
{
    public class BaseElasticEntry
    {
        public BaseElasticEntry()
        {
            ID = GeneralHelper.GenerateUID();
            Username = "<none>";
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
        public string ID { get; set; }
        public string Username { get; set; }
        public long Timestamp { get; set; }
        public string TimestampDate => new DateTime(OSLCommon.OSLHelper.Epoch.Ticks).AddMilliseconds(Timestamp).ToUniversalTime().ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz", CultureInfo.InvariantCulture);
    }
}
