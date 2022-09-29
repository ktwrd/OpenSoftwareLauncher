using System;
using System.Collections.Generic;
using System.Text;

namespace OpenSoftwareLauncher.Shared.Distribution
{
    public class AppLaunchOption
    {
        public string Executable { get; set; } = "";
        public string Arguments { get; set; } = "";
        public string Description { get; set; } = "";
        public bool IsDefault { get; set; } = false;

        public eOperatingSystem OperatingSystem { get; set; } = eOperatingSystem.Any;
        public eArchitecture Architecture { get; set; } = eArchitecture.Any;

        public string[] RequredAppIds { get; set; } = Array.Empty<string>();
        public string[] RequredBranches { get; set; } = Array.Empty<string>();
    }
}
