using System;
using System.Collections.Generic;
using System.Text;

namespace OSLCommon.Logging
{
    public class AccountDeleteEntryData : BaseEntryData
    {
        public AccountDeleteEntryData()
            : base()
        {
            Username = "";
            AuditType = AuditType.AccountDelete;
        }
        public string Username { get; set; }
    }
}
