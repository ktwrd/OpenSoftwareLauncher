using OSLCommon.Authorization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        [Description("Username of the account disabled")]
        public string Username { get; set; }
        [Description("Value of Account.Enabled")]
        public bool State { get; set; }
        [Description("Disable Reason (Blank when State is true)")]
        public string Reason { get; set; }
    }
}
