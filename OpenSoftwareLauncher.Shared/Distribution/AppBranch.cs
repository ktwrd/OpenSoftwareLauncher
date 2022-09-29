using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace OpenSoftwareLauncher.Shared.Distribution
{
    public class AppBranch
    {
        public string AppId { get; set; } = "";
        public string DepotId { get; set; } = "";
        public long Timestamp { get; set; }
        public string BranchName { get; set; } = "default";
    }
}
