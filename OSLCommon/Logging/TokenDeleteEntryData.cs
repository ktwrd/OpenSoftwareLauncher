using Newtonsoft.Json.Linq;
using OSLCommon.Authorization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace OSLCommon.Logging
{
    public class TokenDeleteEntryData : BaseEntryData
    {
        public TokenDeleteEntryData()
            : base()
        {
            AuditType = AuditType.TokenDelete;
            Username = "";
            Token = new AccountToken();
            Bulk = false;
        }
        public TokenDeleteEntryData(Account account, AccountToken token)
            : base()
        {
            AuditType = AuditType.TokenDelete;
            Username = account.Username;
            Token = token;
            Bulk = false;
        }
        [Description("Username of the account affected")]
        public string Username { get; set; }
        [Category("Token")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public AccountToken Token { get; set; }
        [Description("Was this due to a bulk token purge? (/admin/user/token/purge/all)")]
        public bool Bulk { get; set; }
    }
}
