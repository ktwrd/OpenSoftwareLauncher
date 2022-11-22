﻿using System;
using System.Collections.Generic;
using System.Text;

namespace OSLCommon.Logging.Elastic
{
    public class AccountDisableEntry : BaseElasticEntry
    {
        public AccountDisableEntry()
            : base()
        {
            TargetUsername = "";
            State = false;
            Reason = "";
            AuditType = AuditType.AccountDisable;
        }
        public AccountDisableEntry(AccountDisableEntryData data)
            : this()
        {
            TargetUsername = data.Username;
            State = data.State;
            Reason = data.Reason;
            AuditType = AuditType.AccountDisable;
        }

        public string TargetUsername { get; set; }
        public bool State { get; set; }
        public string Reason { get; set; }
        public AuditType AuditType { get; set; }
    }
}
