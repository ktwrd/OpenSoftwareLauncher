using OSLCommon.AutoUpdater;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace OSLCommon.Logging
{
    public class PublishReleaseEntryData : BaseEntryData
    {
        public PublishReleaseEntryData()
            : base()
        {
            AuditType = AuditType.PublishRelease;
            CommitHash = "";
            Signature = "";
        }
        public PublishReleaseEntryData(PublishedRelease published)
        {
            AuditType = AuditType.PublishRelease;
            CommitHash = published.CommitHash;
            Signature = published.Release.remoteLocation;
        }
        [Description("Commit hash of release")]
        public string CommitHash { get; set; }
        [Description("Signature of release")]
        public string Signature { get; set; }
    }
}
