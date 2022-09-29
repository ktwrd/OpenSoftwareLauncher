using System;
using System.Collections.Generic;
using System.Text;

namespace OpenSoftwareLauncher.Shared.Distribution
{
    public enum AppPackageBillingType
    {
        NoCost,
        CDKey
    }
    public enum AppPackageStatus
    {
        Available,
        NotAvailable 
    }
    public class AppPackage
    {
        public string Name { get; set; } = "";
        public string PackageId { get; set; } = "";

        public string[] DepotIds { get; set; } = Array.Empty<string>();
        public string[] AppIds { get; set; } = Array.Empty<string>();

        public AppPackageBillingType BillingType { get; set; }
        public AppPackageStatus Status { get; set; }
    }
}
