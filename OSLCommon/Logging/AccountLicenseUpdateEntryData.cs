using OSLCommon.Authorization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace OSLCommon.Logging
{
    public class AccountLicenseUpdateEntryData : BaseEntryData
    {
        public AccountLicenseUpdateEntryData()
            : base()
        {
            AuditType = AuditType.AccountLicenseUpdate;
            Username = "";
            Removed = Array.Empty<string>();
            Added = Array.Empty<string>();
        }

        public AccountLicenseUpdateEntryData(Account account, string[] previous, string[] current)
            : base()
        {
            AuditType = AuditType.AccountLicenseUpdate;
            Username = account.Username;
            List<string> removeList = new List<string>();
            foreach (var item in previous)
                if (!current.Contains(item))
                    removeList.Add(item);
            List<string> addList = new List<string>();
            foreach (var item in current)
                if (!previous.Contains(item))
                    addList.Add(item);
            Removed = removeList.ToArray();
            Added = addList.ToArray();
        }
        [Description("Username of the account affected")]
        public string Username { get; set; }
        [Description("Licenses removed")]
        public string[] Removed { get; set; }
        [Description("Licenses added")]
        public string[] Added { get; set; }
    }
}
