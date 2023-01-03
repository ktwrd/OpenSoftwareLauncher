using kate.FastConfig;
using OSLCommon.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSLCommon
{
    [ConfigSerialize]
    public class ServerConfig
    {
        [Group("General")]
        public bool AnnouncementEnable { get; set; }
        [Group("Connection")]
        public string MongoDBServer { get; set; }
        public ServerSecurity Security { get; set; }
        public ServerAuth Auth { get; set; }
        public ServerTelemetry Telemetry { get; set; }
        public ServerMongoDB MongoDB { get; set; }
        public ServerMigrate Migrated { get; set; }
    }
}
namespace OSLCommon.Config
{
    [ConfigSerialize]
    [Group("Security")]
    public class ServerSecurity
    {
        public bool AllowAdminOverride { get; set; }
        public bool AllowPermissionReadReleaseBypass { get; set; }
        public bool AllowGroupRestriction { get; set; }
        public bool RequireAuthentication { get; set; }
        public string DefaultSignatures { get; set; }
        public string ImmuneUsers { get; set; }
    }
    [ConfigSerialize]
    [Group("Authentication")]
    public class ServerAuth
    {
        public string Provider { get; set; }
        public string ProviderSignupURL { get; set; }
    }
    [ConfigSerialize]
    [Group("Telemetry")]
    public class ServerTelemetry
    {
        public bool Prometheus { get; set; }
    }
    [ConfigSerialize]
    [Group("MongoDB")]
    public class ServerMongoDB
    {
        public string DatabaseName { get; set; }
        public string Collection_Account { get; set; }
        public string Collection_Announcement {get;set;}
        public string Collection_License {get;set;}
        public string Collection_GroupLicense {get;set;}
        public string Collection_ReleaseInfo {get;set;}
        public string Collection_Published {get;set;}
        public string Collection_AuditLog {get;set;}
        public string Collection_Features {get;set;}
    }
    [ConfigSerialize]
    [Group("Migrated")]
    public class ServerMigrate
    {
        public bool Account { get; set; }
        public bool Announcement { get; set; }
        public bool License { get; set; }
        public bool ReleaseInfo { get; set; }
        public bool Published { get; set; }
    }
}
