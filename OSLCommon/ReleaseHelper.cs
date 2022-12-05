using OSLCommon.AutoUpdater;
using System.Collections.Generic;
using System.Linq;

namespace OSLCommon
{
    public static class ReleaseHelper
    {
        public static ReleaseType ReleaseTypeFromSignature(string signature)
        {
            var recognitionMap = new Dictionary<ReleaseType, string[]>()
                    {
                        {ReleaseType.Beta, new string[] {
                            "-beta",
                            "-canary"
                        } },
                        {ReleaseType.Nightly, new string[] {
                            "-dev",
                            "-devel",
                            "-debug",
                            "-nightly"
                        } },
                        {ReleaseType.Stable, new string[] {
                            "-stable",
                            "-public",
                            "-prod",
                            "-main"
                        } }
                    };
            var targetReleaseType = ReleaseType.Invalid;

            foreach (var pair in recognitionMap)
            {
                var pairTarget = ReleaseType.Invalid;
                foreach (var item in pair.Value)
                {
                    if (pairTarget == ReleaseType.Invalid && (signature.Split('/')[1].EndsWith(item) || signature.EndsWith(item)))
                        pairTarget = pair.Key;
                }
                if (pairTarget != ReleaseType.Invalid)
                {
                    targetReleaseType = pairTarget;
                    break;
                }
            }
            if (signature.Split('/')[1].Split('-').Length == 1 || signature.Split('-').Length == 1)
                targetReleaseType = ReleaseType.Stable;
            else if (targetReleaseType == ReleaseType.Invalid)
                targetReleaseType = ReleaseType.Other;
            return targetReleaseType;
        }
        public static Dictionary<string, ProductRelease> TransformReleaseList(ReleaseInfo[] releases)
        {
            var products = new Dictionary<string, List<ProductReleaseStream>>();
            var productIDLink = new Dictionary<string, List<string>>();

            foreach (var release in releases)
            {
                if (release.executable == null || release.appID == null || release.appID.Length < 1) continue;

                var executable = new ProductExecutable()
                {
                    Linux = release.executable["linux"],
                    Windows = release.executable["windows"]
                };
                if (release.releaseType == ReleaseType.Other)
                {
                    release.releaseType = ReleaseTypeFromSignature(release.remoteLocation);
                }
                var stream = new ProductReleaseStream()
                {
                    ProductName = release.productName,
                    ProductVersion = release.version,
                    ProductExpiryTimestamp = 0,
                    BranchName = release.releaseType.ToString(),
                    UpdatedTimestamp = release.envtimestamp,
                    RemoteSignature = release.remoteLocation,
                    Executable = executable,
                    ProductID = release.appID,
                    CommitHash = release.commitHash
                };
                if (!products.ContainsKey(release.appID))
                    products.Add(release.appID, new List<ProductReleaseStream>());
                if (!productIDLink.ContainsKey(release.appID))
                    productIDLink.Add(release.appID, new List<string>());
                if (!productIDLink[release.appID].Contains(release.productName))
                    productIDLink[release.appID].Add(release.productName);
                products[release.appID].Add(stream);
            }

            var releaseTable = new Dictionary<string, ProductRelease>();
            foreach (var pair in products)
            {
                var release = new ProductRelease()
                {
                    ProductName = productIDLink[pair.Key].FirstOrDefault(),
                    ProductID = pair.Key,
                    Streams = pair.Value.ToArray()
                };
                releaseTable.Add(pair.Key, release);
            }

            return releaseTable;
        }
    }
}
