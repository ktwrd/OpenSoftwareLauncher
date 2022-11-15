using System;
using System.Collections.Generic;
using System.Text;

namespace OSLCommon.Logging
{
    public class BulkTokenDeleteEntryData : BaseEntryData
    {
        public BulkTokenDeleteEntryData()
            : base()
        {
            AuditType = AuditType.BulkTokenDelete;
            Dict = new Dictionary<string, int>();
        }

        public Dictionary<string, int> Dict { get; set; }
        public int Total { get; set; }
        public override string SerializeToJSON()
        {
            Total = 0;
            foreach (var thing in Dict)
                Total += thing.Value;
            return base.SerializeToJSON();
        }
    }
}
