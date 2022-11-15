using System;
using System.Collections.Generic;
using System.Text;

namespace OSLCommon.Logging
{
    public class AccountModifyEntryData : BaseEntryData
    {
        public AccountModifyEntryData()
            : base()
        {
            AuditType = AuditType.AccountModify;
            Username = "";
            Field = "";
            Previous = "";
            Current = "";
        }

        public string Username { get; set; }
        public string Field { get; set; }
        public object Previous { get; set; }
        public object Current { get; set; }
    }
}
