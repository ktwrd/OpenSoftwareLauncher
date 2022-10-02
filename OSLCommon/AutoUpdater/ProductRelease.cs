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

        #region bFirebaseSerializable
        /*public async Task FromFirebase(DocumentSnapshot document, VoidDelegate completeIncrement)
        {
            this.UID = document.Reference.Id;

            this.ProductName = FirebaseHelper.ParseString(document, "ProductName");
            this.ProductID = FirebaseHelper.ParseString(document, "ProductID");

            var dict = document.ToDictionary();
            var streamList = new List<ProductReleaseStream>();
            var taskList = new List<Task>();
            if (dict.ContainsKey("Streams"))
            {
                foreach (object fz in (List<object>)dict["Streams"])
                {
                    taskList.Add(new Task(new Action(delegate
                    {
                        var f = (DocumentReference)fz;
                        var res = FirebaseHelper.DeserializeDocumentReference<ProductReleaseStream>(f, completeIncrement);
                        res.Wait(TimeSpan.FromSeconds(15));
                        if (res.Result != null)
                            streamList.Add(res.Result);
                    })));
                }
            }
            foreach (var i in taskList)
                i.Start();
            await Task.WhenAll(taskList.ToArray());
            this.Streams = streamList.ToArray();
            completeIncrement();
        }
        public async Task ToFirebase(DocumentReference document, VoidDelegate completeIncrement)
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                { "ProductName", ProductName },
                { "ProductID", ProductID }
            };
            var refList = new List<DocumentReference>();
            foreach (var stream in Streams)
            {
                var refr = stream.GetFirebaseDocumentReference(document.Database);
                await stream.ToFirebase(refr, completeIncrement);
                refList.Add(refr);
            }
            data.Add("Streams", refList);
            await document.SetAsync(data);
            completeIncrement();
        }
        public DocumentReference GetFirebaseDocumentReference(FirestoreDb database) => database.Document(FirebaseHelper.FirebaseCollection[this.GetType()] + "/" + UID);*/
        #endregion

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
