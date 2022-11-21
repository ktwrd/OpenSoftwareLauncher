using MongoDB.Driver.Core.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSLCommon.Authorization
{
    public class ServiceAccountCreateRequest
    {
        public ServiceAccountCreateRequest()
        {
            Username = "";
            Permissions = Array.Empty<AccountPermission>();
            Licenses = Array.Empty<string>();
        }
        public string Username { get; set; }
        public AccountPermission[] Permissions { get; set; }
        public string[] Licenses { get; set; }
    }
}
