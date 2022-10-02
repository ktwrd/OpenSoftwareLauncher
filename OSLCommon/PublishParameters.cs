using OSLCommon.AutoUpdater;
using System.Collections.Generic;

namespace OSLCommon
{
    public class PublishParameters
    {
        public string token { get; set; }
        public string organization { get; set; }
        public string product { get; set; }
        public string branch { get; set; }
        public long timestamp { get; set; }
        public ReleaseInfo releaseInfo { get; set; }
        public List<ManagedUploadSendData> files = new List<ManagedUploadSendData>();

        public PublishParameters()
        {
            token = "";
            organization = "";
            product = "";
            branch = "";
            timestamp = 0;
            releaseInfo = ReleaseInfo.Blank();
        }
    }

    public class ManagedUploadSendData
    {
        public string Location = "";
        public string ETag = "";
        public string Bucket = "";
        public string Key = "";

        public PublishedReleaseFile ToPublishedReleaseFile(string commitHash)
        {
            var prf = new PublishedReleaseFile()
            {
                Location = Location,
                CommitHash = commitHash
            };
            if (Location.EndsWith(@"win-amd64.exe") || Location.EndsWith("setup.exe"))
            {
                prf.Platform = FilePlatform.Windows;
                prf.Type = FileType.Installer;
            }
            else if (Location.EndsWith(@"win-amd64.zip"))
            {
                prf.Platform = FilePlatform.Windows;
                prf.Type = FileType.Portable;
            }
            else if (Location.EndsWith(@".tar.gz") || Location.EndsWith(@"linux-amd64.tar.gz"))
            {
                prf.Platform = FilePlatform.Linux;
                prf.Type = FileType.Portable;
            }
            return prf;
        }
    }
}
