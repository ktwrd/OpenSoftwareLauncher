using Google.Cloud.Firestore;
using kate.shared.Helpers;
using kate.shared.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OSLCommon.AutoUpdater
{
    public interface IProductReleaseStream
    {
        string UID { get; }
        string ProductID { get; set; }
        string ProductName { get; set; }
        string ProductVersion { get; set; }
        long ProductExpiryTimestamp { get; set; }
        string BranchName { get; set; }
        long UpdatedTimestamp { get; set; }
        string RemoteSignature { get; set; }
        ProductExecutable Executable { get; set; }
        string CommitHash { get; set; }
    }

    [Serializable]
    public class ProductReleaseStream : bSerializable, IProductReleaseStream
    {
        public string UID { get; private set; }

        public string ProductID { get; set; }

        public string ProductName { get; set; }
        public string ProductVersion { get; set; }
        public long ProductExpiryTimestamp { get; set; }
        public DateTimeOffset ProductExpiryAt => DateTimeOffset.FromUnixTimeMilliseconds(ProductExpiryTimestamp);

        public string BranchName { get; set; }
        public long UpdatedTimestamp { get; set; }
        public DateTimeOffset UpdatedAt => DateTimeOffset.FromUnixTimeMilliseconds(UpdatedTimestamp);

        public string RemoteSignature { get; set; }
        public ProductExecutable Executable { get; set; }
        public string CommitHash { get; set; }

        public ProductReleaseStream()
        {
            UID = GeneralHelper.GenerateUID();
            ProductID = "";
            ProductName = "";
            ProductVersion = "";
            ProductExpiryTimestamp = 0;
            BranchName = "";
            UpdatedTimestamp = 0;
            RemoteSignature = "";
            Executable = new ProductExecutable();
            CommitHash = "";
        }
        #region bSerializable
        public void ReadFromStream(SerializationReader sr)
        {
            ProductID = sr.ReadString();
            ProductName = sr.ReadString();
            ProductVersion = sr.ReadString();
            ProductExpiryTimestamp = sr.ReadInt64();

            BranchName = sr.ReadString();
            UpdatedTimestamp = sr.ReadInt64();

            RemoteSignature = sr.ReadString();
            Executable = (ProductExecutable)sr.ReadObject();
            CommitHash = sr.ReadString();
        }
        public void WriteToStream(SerializationWriter sw)
        {
            sw.Write(ProductID);
            sw.Write(ProductName);
            sw.Write(ProductVersion);
            sw.Write(ProductExpiryTimestamp);

            sw.Write(BranchName);
            sw.Write(UpdatedTimestamp);

            sw.Write(RemoteSignature);
            sw.WriteObject(Executable);

            sw.Write(CommitHash);
        }
        #endregion
    }
}
