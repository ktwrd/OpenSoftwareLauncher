using OSLCommon.Authorization;
using System;
using System.Collections.Generic;
using System.Security;
using System.Text;

namespace OSLCommon.Logging.Elastic
{
    public class AccountPermissionRevokeEntry : BaseElasticEntry
    {
        public AccountPermissionRevokeEntry()
            : base()
        {
            TargetUsername = "<none>";
            Permission = AccountPermission.INVALID;
            AuditType = AuditType.AccountPermissionRevoke;
        }
        public string TargetUsername { get; set; }

        public AccountPermission Permission { get; set; }
        public AuditType AuditType { get; set; }
    }
}
