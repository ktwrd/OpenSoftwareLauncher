using kate.FastConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSoftwareLauncher.DesktopWinForms
{
    public class Config
    {
        [Group("General")]
        public string Language { get; set; }
        [Group("General")]
        public bool ShowLatestRelease { get; set; }
        [Group("Connection")]
        public string Endpoint { get; set; }
        public ConfigAuth Auth { get; set; }
        public ConfigAuditLog AuditLog { get; set; }
    }
    [ConfigSerialize]
    [Group("Authentication")]
    public class ConfigAuth
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public bool Remember { get; set; }
    }
    [ConfigSerialize]
    [Group("AuditLog")]
    public class ConfigAuditLog
    {
        public ConfigAuditLog()
        {
            DefaultTimeRange_MinOffset = -86400000;
        }
        public long DefaultTimeRange_MinOffset { get; set; }
    }
}
