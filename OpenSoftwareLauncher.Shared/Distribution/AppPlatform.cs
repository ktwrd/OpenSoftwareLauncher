using System;
using System.Collections.Generic;
using System.Text;

namespace OpenSoftwareLauncher.Shared.Distribution
{
    public class AppPlatform
    {
        public bool Windows { get; set; } = false;
        public bool Windows64BitOnly { get; set; } = false;

        public bool Darwin { get; set; } = false;
        public bool Darwin_64BitIntel { get; set; } = false;
        public bool Darwin_Silicon { get; set; } = false;
        public bool Darwin_Notarized { get; set; } = false;

        public bool Linux { get; set; } = false;
    }
}
