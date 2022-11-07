using OSLCommon.AutoUpdater;
using kate.shared.Helpers;
using System;
using System.Collections.Generic;
using MongoDB.Bson;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace OSLCommon
{
    [Serializable]
    public class PublishedRelease : bSerializable
    {
        [JsonIgnore]
        [XmlIgnore]
        [SoapIgnore]
        public ObjectId _id { get; set; }
        public string UID { get; set; }
        public string CommitHash { get; set; }
        public long Timestamp { get; set; }
        public ReleaseInfo Release { get; set; }
        public PublishedReleaseFile[] Files { get; set; }

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
        public PublishedRelease()
        {
            UID = GeneralHelper.GenerateUID();
            CommitHash = "";
            Timestamp = 0;
            Release = ReleaseInfo.Blank();
            Files = Array.Empty<PublishedReleaseFile>();
        }
    }
    [Serializable]
    public class PublishedReleaseFile : bSerializable
    {
        [JsonIgnore]
        [XmlIgnore]
        [SoapIgnore]
        public ObjectId _id { get; set; }
        public string UID { get; set; }
        public string Location { get; set; }
        public string CommitHash { get; set; }
        public FilePlatform Platform { get; set; }
        public FileType Type { get; set; }
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
