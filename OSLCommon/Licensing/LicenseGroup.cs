using Google.Apis.Http;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace OSLCommon.Licensing
{
    public class LicenseGroup
    {
        [JsonIgnore]
        [XmlIgnore]
        [SoapIgnore]
        public ObjectId _id { get; set; }
        internal AccountLicenseManager manager;
        internal bool eventHook = false;
        private string uid = "";
        public string UID
        {
            get => uid;
            set
            {
                uid = value;
                if (manager != null)
                    manager.OnGroupUpdate(this);
            }
        }
        private string displayName = "";
        public string DisplayName
        {
            get => displayName;
            set
            {
                displayName = value;
                if (manager != null)
                    manager.OnGroupUpdate(this);
            }
        }
        [Browsable(false)]
        private string[] licenseIds = Array.Empty<string>();
        public string[] LicenseIds
        {
            get => licenseIds;
            set
            {
                licenseIds = value;
                if (manager != null)
                    manager.OnGroupUpdate(this);
            }
        }
        private long createdTimestamp = 0;
        public long CreatedTimestamp
        {
            get => createdTimestamp;
            set
            {
                createdTimestamp = value;
                if (manager != null)
                    manager.OnGroupUpdate(this);
            }
        }
        private string createdBy = "";
        public string CreatedBy
        {
            get => createdBy;
            set
            {
                createdBy = value;
                if (manager != null)
                    manager.OnGroupUpdate(this);
            }
        }
        private string note = "";
        public string Note
        {
            get => note;
            set
            {
                note = value;
                if (manager != null)
                    manager.OnGroupUpdate(this);
            }
        }

        internal void Merge(LicenseGroup group)
        {
            uid = group.uid;
            displayName = group.displayName;
            licenseIds = group.licenseIds;
            createdTimestamp = group.createdTimestamp;
            createdBy = group.createdBy;
            note = group.note;
        }
    }
}
