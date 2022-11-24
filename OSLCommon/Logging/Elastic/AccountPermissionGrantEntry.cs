using OSLCommon.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSLCommon.Logging.Elastic
{
    public class AccountPermissionGrantEntry : BaseElasticEntry
    {
        public AccountPermissionGrantEntry()
            : base()
        {
            AuditType = AuditType.AccountPermissionGrant;
            Permission = AccountPermission.INVALID;
            TargetUsername = "<none>";
        }

        public string TargetUsername { get; set; }
        public AccountPermission Permission { get; set; }
        public AuditType AuditType { get; set; }
    }
}
