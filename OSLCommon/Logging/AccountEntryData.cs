using OSLCommon.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSLCommon.Logging
{
    public class AccountDisableEntryData : BaseEntryData
    {
        public AccountDisableEntryData()
            : base()
        {
            Username = "";
            AuditType = AuditType.AccountDisable;
            State = true;
            Reason = "";
        }
        public AccountDisableEntryData(Account account)
            : this()
        {
            Username = account.Username;
            State = account.Enabled;
            foreach (var i in account.disableReasons)
                Reason = i.Message;
        }
        public string Username { get; set; }
        public bool State { get; set; }
        public string Reason { get; set; }
    }
}
