using OSLCommon;
using OSLCommon.Authorization;
using OSLCommon.AutoUpdater;
using OSLCommon.Licensing;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenSoftwareLauncher.Server
{
    public interface IContentManager
    {
        List<ReleaseInfo> ReleaseInfoContent { get; set; }
        Dictionary<string, ProductRelease> Releases { get; set; }
        Dictionary<string, PublishedRelease> Published { get; set; }
        AccountManager AccountManager { get; set; }
        SystemAnnouncement SystemAnnouncement { get; set; }
        List<LicenseKeyMetadata> LicenseKeys { get; set; }
        Dictionary<string, string> LicenseKeyGroupNote { get; set; }
    }
}
