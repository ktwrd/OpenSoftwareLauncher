using OSLCommon.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSLCommon.Logging
{
    public class AccountPermissionRevokeEntryData : BaseEntryData
    {
        public AccountPermissionRevokeEntryData()
            : base()
        {
            AuditType = AuditType.AccountPermissionRevoke;
            Username = "";
            Permission = (AccountPermission)(-1);
        }
        public AccountPermissionRevokeEntryData(Account account, AccountPermission permission)
            : base()
        {
            AuditType = AuditType.AccountPermissionRevoke;
            Username = account.Username;
            Permission = permission;
        }

        public string Username { get; set; }
        public AccountPermission Permission { get; set; }
    }
}
