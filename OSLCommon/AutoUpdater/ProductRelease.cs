using Google.Cloud.Firestore;
using kate.shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSLCommon.AutoUpdater
{
    public interface IProductRelease
    {
        string UID { get; set; }
        string ProductName { get; set; }
        string ProductID { get; set; }
        ProductReleaseStream[] Streams { get; set; }
    }

    [Serializable]
    public class ProductRelease : bSerializable, IProductRelease
    {
        public string UID { get; set; }
        public string ProductName { get; set; }
        public string ProductID { get; set; }
        public ProductReleaseStream[] Streams { get; set; }

        public ProductRelease()
        {
            UID = GeneralHelper.GenerateUID();
            ProductName = "";
            ProductID = "";
            Streams = Array.Empty<ProductReleaseStream>();
        }

        #region bSerializable
        public void ReadFromStream(SerializationReader sr)
        {
            ProductName = sr.ReadString();
            ProductID = sr.ReadString();
            Streams = ((List<ProductReleaseStream>)sr.ReadBList<ProductReleaseStream>()).ToArray();
        }
        public void WriteToStream(SerializationWriter sw)
        {
            sw.Write(ProductName);
            sw.Write(ProductID);
            sw.Write<ProductReleaseStream>(new List<ProductReleaseStream>(Streams));
        }
        #endregion
    }
}
