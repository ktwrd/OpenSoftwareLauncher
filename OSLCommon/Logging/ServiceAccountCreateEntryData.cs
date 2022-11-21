using OSLCommon.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSLCommon.Logging
{
    public class ServiceAccountCreateEntryData : BaseEntryData
    {
        public ServiceAccountCreateEntryData()
            : base()
        {
            Username = "";
            Permissions = Array.Empty<AccountPermission>();
            Licenses = Array.Empty<string>();
        }

        public ServiceAccountCreateEntryData(Account account)
            : base()
        {
            Username = account.Username;
            Permissions = account.Permissions;
            Licenses = account.Licenses;
        }

        public string Username { get; set; }
        public AccountPermission[] Permissions { get; set; }
        public string[] Licenses { get; set; }
    }
}
