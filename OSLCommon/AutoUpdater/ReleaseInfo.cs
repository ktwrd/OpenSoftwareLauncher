using Google.Cloud.Firestore;
using kate.shared.Helpers;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OSLCommon.AutoUpdater
{
    public enum ReleaseType
    {
        Invalid = -1,
        Stable,
        Beta,
        Nightly,
        Other
    }
    public interface IReleaseInfo
    {
        string UID { get; set; }
        string version { get; set; }
        string name { get; set; }
        string productName { get; set; }
        string appID { get; set; }
        long timestamp { get; set; }
        long envtimestamp { get; set; }
        string remoteLocation { get; set; }
        string commitHash { get; set; }
        string commitHashShort { get; set; }
        ReleaseType releaseType { get; set; }
        Dictionary<string, string> files { get; set; }
        Dictionary<string, string> executable { get; set; }
    }
    [Serializable]
    public class ReleaseInfo : IReleaseInfo, bSerializable
    {
        [JsonIgnore]
        [XmlIgnore]
        [SoapIgnore]
        public ObjectId _id { get; set; }
        public string UID { get; set; }
        public string version { get; set; }
        public string name { get; set; }
        public string productName { get; set; }
        public string appID { get; set; }
        public long timestamp { get; set; }
        public long envtimestamp { get; set; }
        public string remoteLocation { get; set; }
        public string commitHash { get; set; }
        public string commitHashShort { get; set; }
        public ReleaseType releaseType { get; set; }
        public Dictionary<string, string> files { get; set; }
        public Dictionary<string, string> executable { get; set; }
        public static ReleaseInfo Blank()
        {
            return new ReleaseInfo();
        }
        public ReleaseInfo()
        {
            UID = GeneralHelper.GenerateUID();
            version = @"";
            name = @"";
            productName = @"";
            appID = @"";
            timestamp = 0;
            envtimestamp = 0;
            remoteLocation = @"";
            commitHash = @"";
            commitHashShort = @"";
            releaseType = ReleaseType.Other;
            files = new Dictionary<string, string>();
            executable = new Dictionary<string, string>();
        }

        #region bSerializable
        public void ReadFromStream(SerializationReader sr)
        {
            version = sr.ReadString();
            name = sr.ReadString();
            productName = sr.ReadString();
            appID = sr.ReadString();
            timestamp = sr.ReadInt64();
            envtimestamp = sr.ReadInt64();
            remoteLocation = sr.ReadString();
            commitHash = sr.ReadString();
            commitHashShort = sr.ReadString();
            releaseType = (ReleaseType)sr.ReadInt32();
            files = (Dictionary<string, string>)sr.ReadDictionary<string, string>();
            executable = (Dictionary<string, string>)sr.ReadDictionary<string, string>();
            UID = sr.ReadString();
        }
        public void WriteToStream(SerializationWriter sw)
        {
            sw.Write(version);
            sw.Write(name);
            sw.Write(productName);
            sw.Write(appID);
            sw.Write(timestamp);
            sw.Write(envtimestamp);
            sw.Write(remoteLocation);
            sw.Write(commitHash);
            sw.Write(commitHashShort);
            sw.Write(Convert.ToInt32(releaseType));
            sw.Write(files);
            sw.Write(executable);
            sw.Write(UID);
        }
        #endregion
    }
}
