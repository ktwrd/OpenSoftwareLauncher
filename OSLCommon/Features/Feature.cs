using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace OSLCommon.Features
{
    public class Feature
    {
        [JsonIgnore]
        [XmlIgnore]
        [SoapIgnore]
        [Browsable(false)]
        public ObjectId _id { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public Feature()
        {
            Name = "";
            URL = "";
        }
    }
}
