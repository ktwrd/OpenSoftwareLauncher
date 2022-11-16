using OSLCommon.Authorization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        [Description("Username of the account affected")]
        public string Username { get; set; }
        [Description("Permission granted to user")]
        public AccountPermission Permission { get; set; }
        [Description("Was this permission granted due to the account activating a License Key?")]
        public bool FromLicense { get; set; }
    }
}
