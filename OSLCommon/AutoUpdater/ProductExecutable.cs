using Google.Cloud.Firestore;
using kate.shared.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OSLCommon.AutoUpdater
{
    public interface IProductExecutable
    {
        string UID { get; }
        string Linux { get; set; }
        string Windows { get; set; }
    }
    [Serializable]
    public class ProductExecutable : bSerializable, IProductExecutable
    {
        public string UID { get; private set; }
        public string Linux { get; set; }
        public string Windows { get; set; }

        public ProductExecutable()
        {
            UID = GeneralHelper.GenerateUID();
            Linux = "";
            Windows = "";
        }

        #region bSerializable
        public void ReadFromStream(SerializationReader sr)
        {
            Linux = sr.ReadString();
            Windows = sr.ReadString();
        }
        public void WriteToStream(SerializationWriter sw)
        {
            sw.Write(Linux);
            sw.Write(Windows);
        }
        #endregion

    }
}
