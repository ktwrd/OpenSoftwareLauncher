using Newtonsoft.Json.Linq;
using OSLCommon.Authorization;
using System;
using System.Collections.Generic;
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
        public string Username { get; set; }
        public AccountToken Token { get; set; }
        public bool Bulk { get; set; }
    }
}
