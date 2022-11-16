using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private Dictionary<string, int> dict = new Dictionary<string, int>();
        [Description("Username of the account (key) and the tokens removed (value)")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Dictionary<string, int> Dict
        {
            get => dict;
            set
            {
                dict = value;
                Total = 0;
                foreach (var thing in dict)
                    Total += thing.Value;
            }
        }
        [Description("Total amount of tokens deleted.")]
        public int Total { get; set; }
    }
}
