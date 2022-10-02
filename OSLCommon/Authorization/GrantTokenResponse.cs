using System;
using System.Collections.Generic;
using System.Text;

namespace OSLCommon.Authorization
{
    public class GrantTokenResponse
    {
        public GrantTokenResponse(string message = "", bool success = false, AccountToken token = null, string[] groups=null, AccountPermission[] permissions = null)
        {
            Message = message;
            Token = token;
            Success = success;
            Groups = groups == null ? new string[] { } : groups;
            Permissions = permissions == null ? new AccountPermission[] { } : permissions;
        }
        public bool Success { get; private set; }
        public string Message { get; private set; }

        public string[] Groups { get; private set; }
        public AccountPermission[] Permissions { get; private set; }

        /// <summary>
        /// Nullable
        /// </summary>
        public AccountToken Token { get; private set; }
    }
}
