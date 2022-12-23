using kate.shared.Helpers;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace OSLCommon.Authorization
{
    public class PermissionGroup
    {
        public PermissionGroup()
        {
            UID = GeneralHelper.GenerateUID();
            Name = "<none>";
            Permissions = Array.Empty<AccountPermission>();
        }
        [Browsable(false)]
        public ObjectId _id { get; set; }
        public string UID { get; set; }
        public string Name { get; set; }
        public AccountPermission[] Permissions { get; set; }
    }
}
