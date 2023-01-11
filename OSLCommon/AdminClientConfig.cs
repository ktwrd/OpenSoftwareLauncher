using kate.FastConfig;
using OSLCommon.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSLCommon
{
    public class AdminClientConfig
    {
        [Group("General")]
        public string Language { get; set; }
        [Group("General")]
        public bool ShowLatestRelease { get; set; }
        [Group("Connection")]
        public string Endpoint { get; set; }
        public AdminClientAuth Auth { get; set; }
        public AdminAuditLog AuditLog { get; set; }
        public AdminClientConfig()
        {
            Language = "en";
            ShowLatestRelease = true;
            Endpoint = "";
        }
    }
}
namespace OSLCommon.Config
{
    [ConfigSerialize]
    [Group("Authentication")]
    public class AdminClientAuth
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public bool Remember { get; set; }
        public AdminClientAuth()
        {
            Username = "";
            Token = "";
            Remember = false;
        }
    }
    [ConfigSerialize]
    [Group("AuditLog")]
    public class AdminAuditLog
    {
        public long DefaultTimeRange_MinOffset { get; set; }
        public AdminAuditLog()
        {
            DefaultTimeRange_MinOffset = -86400000;
        }
    }
}
