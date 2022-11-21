using System;
using System.Collections.Generic;
using System.Text;

namespace OSLCommon.Authorization
{
    public class ServiceAccountCreateRequest
    {
        public string Usernamme { get; set; }
        public AccountPermission[] Permissions { get; set; }
        public string[] Licenses { get; set; }
    }
}
