using OSLCommon.AutoUpdater;
using Google.Cloud.Firestore;
using kate.shared.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OSLCommon
{
    [Serializable]
    public class PublishedRelease : bSerializable
    {
        public string UID { get; set; } = GeneralHelper.GenerateUID();
        public string CommitHash { get; set; } = "";
        public long Timestamp { get; set; } = 0;
        public ReleaseInfo Release { get; set; } = ReleaseInfo.Blank();
        public PublishedReleaseFile[] Files { get; set; } = Array.Empty<PublishedReleaseFile>();

        public void ReadFromStream(SerializationReader sr)
        {
            CommitHash = sr.ReadString();
            Timestamp = sr.ReadInt64();
            Release = (ReleaseInfo)sr.ReadObject();
            Files = new List<PublishedReleaseFile>(sr.ReadBList<PublishedReleaseFile>()).ToArray();
        }
        public void WriteToStream(SerializationWriter sw)
        {
            sw.Write(CommitHash);
            sw.Write(Timestamp);
            sw.WriteObject(Release);
            sw.Write(new List<PublishedReleaseFile>(Files));
        }
    }
    [Serializable]
    public class PublishedReleaseFile : bSerializable
    {
        public string UID { get; set; } = GeneralHelper.GenerateUID();
        public string Location { get; set; } = "";
        public string CommitHash { get; set; } = "";
        public FilePlatform Platform { get; set; } = FilePlatform.Any;
        public FileType Type { get; set; } = FileType.Other;
        public void ReadFromStream(SerializationReader sr)
        {
            Location = sr.ReadString();
            CommitHash = sr.ReadString();
            Platform = (FilePlatform)sr.ReadInt32();
            Type = (FileType)sr.ReadInt32();
        }
        public void WriteToStream(SerializationWriter sw)
        {
            sw.Write(Location);
            sw.Write(CommitHash);
            sw.Write(Convert.ToInt32(Platform));
            sw.Write(Convert.ToInt32(Type));
        }
        public PublishedReleaseFile()
        {
            UID = GeneralHelper.GenerateUID();
            Location = "";
            CommitHash = "";
            Platform = FilePlatform.Any;
            Type = FileType.Other;
        }
    }
    public enum FileType
    {
        Other,
        Portable,
        Installer
    }
    public enum FilePlatform
    {
        Any,
        Windows,
        Linux
    }
}
