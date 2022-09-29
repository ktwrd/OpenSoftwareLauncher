using System;
using System.Collections.Generic;
using System.Text;

namespace OpenSoftwareLauncher.Shared.Authentication
{
    public class User
    {
        public string Username { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public long CreatedTimestamp { get; set; } = 0;

        public string[] PackageIds { get; set; } = Array.Empty<string>();
    }
}
