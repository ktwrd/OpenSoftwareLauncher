using OSLCommon.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSLCommon.Logging
{
    public class TokenCreateEntryData : BaseEntryData
    {
        public TokenCreateEntryData()
            : base()
        {
            AuditType = AuditType.TokenCreate;
            Username = "";
            Token = new AccountToken();
        }
        public TokenCreateEntryData(Account account, AccountToken token)
            : base()
        {
            AuditType = AuditType.TokenCreate;
            Username = account.Username;
            Token = token;
        }
        public string Username { get; set; }
        public AccountToken Token { get; set; }
    }
}
