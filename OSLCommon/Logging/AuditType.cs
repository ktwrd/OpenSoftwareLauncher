using System;
using System.Collections.Generic;
using System.Text;

namespace OSLCommon.Logging
{
    public enum AuditType
    {
        None,
        
        AccountModify           = 100,
        AccountDisable          = 101,
        AccountPermissionGrant  = 120,
        AccountPermissionRevoke = 121,
        LicenseRedeem           = 130,
        AccountLicenseUpdate    = 131,
        TokenDelete             = 140,
        TokenCreate             = 142,

        AnnouncementModify      = 200,
        AnnouncementDelete      = 201,

        PublishRelease          = 210,

        LicenseCreate   = 300,
        LicenseDisable  = 301,
        LicenseEnable   = 302
    }
}
