using OSLCommon.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSLCommon.Logging
{
    public class AccountPermissionGrantEntryData : BaseEntryData
    {
        public AccountPermissionGrantEntryData()
            : base()
        {
            AuditType = AuditType.AccountPermissionGrant;
            Username = "";
            Permission = (AccountPermission) (-1);
            FromLicense = false;
        }
        public AccountPermissionGrantEntryData(Account account, AccountPermission permission, bool fromLicense)
            : base()
        {
            AuditType = AuditType.AccountPermissionGrant;
            Username = account.Username;
            Permission = permission;
            FromLicense = fromLicense;
        }

        public string Username { get; set; }
        public AccountPermission Permission { get; set; }
        public bool FromLicense { get; set; }
    }
}
