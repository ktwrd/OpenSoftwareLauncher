using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenSoftwareLauncher.Shared.Distribution
{
    public class App
    {
        public string AppId { get; set; } = "";
        public string ParentAppId { get; set; } = "";
        public string LatestDepotId { get; set; } = "";
        public string[] Branches { get; set; } = Array.Empty<string>();

        public AppLaunchOption[] LaunchOptions { get; set; } = Array.Empty<AppLaunchOption>();

        /// <summary>
        /// Nullable
        /// </summary>
        public AppLaunchOption DefaultLaunchOption
        {
            get
            {
                if (LaunchOptions.Length == 1)
                    return LaunchOptions[0];
                var count = LaunchOptions.Where(v => v.IsDefault);
                if (count.Count() < 1)
                    return null;
                return count.First();
            }
        }

        public Dictionary<string, object> Configuration { get; set; } = new Dictionary<string, object>();

        public AppPlatform SupportedPlatforms { get; set; } = new AppPlatform();

        /// <summary>
        /// Is this publicly visible for everyone? When released, anyone can see the manifest but cannot download any content (or view files) without being assigned a package that contains this app.
        /// </summary>
        public bool Released { get; set; } = false;
    }
}
