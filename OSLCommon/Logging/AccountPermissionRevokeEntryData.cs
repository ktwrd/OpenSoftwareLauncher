using OSLCommon.Authorization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        [Description("Username of the account disabled")]
        public string Username { get; set; }
        [Description("Permission revoked from user")]
        public AccountPermission Permission { get; set; }
    }
}
