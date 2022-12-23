using OSLCommon.Authorization;
using OSLCommon.Licensing;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSLCommon.Helpers
{
    public delegate void AccountDelegate(Account account);

    public delegate void LicenseFieldDelegate(LicenseField field, LicenseKeyMetadata license);
    public delegate void LicenseGroupDelegate(LicenseGroup group);
    public delegate void LicenseDelegate(LicenseKeyMetadata license);

    public delegate void AnnouncementDelegate(SystemAnnouncementEntry announcement);

    public delegate void ParameterDelegate<T>(T arg);
}
