using System;
using System.Collections.Generic;
using System.Text;

namespace OSLCommon.Logging.Elastic
{
    public class AccountDeleteEntry : BaseElasticEntry
    {
        public AccountDeleteEntry()
            : base()
        {
            TargetUsername = "<none>";
            AuditType = AuditType.AccountDelete;
        }
        public string TargetUsername { get; set; }
        public AuditType AuditType { get; set; }
    }
}
