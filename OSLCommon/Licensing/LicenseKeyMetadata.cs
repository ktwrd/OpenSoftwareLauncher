using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using OSLCommon.Authorization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace OSLCommon.Licensing
{
    public class LicenseKeyMetadata
    {
        public LicenseKeyMetadata()
            : this(null)
            { }
        public LicenseKeyMetadata(AccountLicenseManager manager)
        {
            this.manager = manager;
        }
        internal AccountLicenseManager manager;
        internal void Merge(LicenseKeyMetadata license)
        {
            enable = license.enable;
            activated = license.activated;
            activatedBy = license.activatedBy;
            activateTimestamp = license.activateTimestamp;
            InternalNote = license.internalNote;
            key = license.key;
            products = license.products;
            productsApplied = license.productsApplied;
            permissions = license.permissions;
            permissionsApplied = license.permissionsApplied;
            activateByTimestamp = license.activateByTimestamp;
            createdTimestamp = license.createdTimestamp;
            createdBy = license.createdBy;
            groupId = license.groupId;
        }
        [JsonIgnore]
        [XmlIgnore]
        [SoapIgnore]
        [BsonIgnore]
        internal bool eventHook = false;
        [JsonIgnore]
        [XmlIgnore]
        [SoapIgnore]
        public ObjectId _id { get; set; }

        [Category("Metadata"), Description("Unique Identifier of the License Key")]
        public string UID { get; set; }

        private bool enable = true;
        /// <summary>
        /// Allow license key to be activated
        /// </summary>
        [Category("Metadata"), Description("Allow license key to be activated")]
        public bool Enable { get => enable; set
            {
                enable = value;
                if (manager != null)
                    manager.OnUpdate(LicenseField.License, this);
            }
        }

        private bool activated = false;
        /// <summary>
        /// Has the license key been activated?
        /// </summary>
        [Category("Metadata"), Description("Has the license key been activated")]
        public bool Activated
        {
            get => activated;
            set
            {
                activated = value;
                if (manager != null)
                    manager.OnUpdate(LicenseField.License, this);
            }
        }

        private string activatedBy = "";
        /// <summary>
        /// Username of the account that activated this license key
        /// </summary>
        [Category("Activator"), Description("Username of the account that activated this license key.")]
        public string ActivatedBy
        {
            get => activatedBy;
            set
            {
                activatedBy = value;
                if (manager != null)
                    manager.OnUpdate(LicenseField.License, this);
            }
        }

        private long activateTimestamp = 0;
        /// <summary>
        /// Timestamp when the license key was activated (milliseconds, UTC unix epoch)
        /// </summary>
        [Category("Activator"), Description("Timestamp when the license key was activated. Milliseconds since UTC Unix Epoch")]
        public long ActivateTimestamp
        {
            get => activateTimestamp;
            set
            {
                activateTimestamp = value;
                if (manager != null)
                    manager.OnUpdate(LicenseField.License, this);
            }
        }

        private string internalNote = "";
        /// <summary>
        /// Internal note for admins
        /// </summary>
        [Category("Metadata")]
        public string InternalNote
        {
            get => internalNote;
            set
            {
                internalNote = value;
                if (manager != null)
                    manager.OnUpdate(LicenseField.License, this);
            }
        }

        private string key = "";
        /// <summary>
        /// Actual License Key
        /// </summary>
        [Category("Metadata"), Description("License Key")]
        public string Key
        {
            get => key;
            set
            {
                key = value;
                if (manager != null)
                    manager.OnUpdate(LicenseField.License, this);
            }
        }

        private string[] products = Array.Empty<string>();
        /// <summary>
        /// Products to activate
        /// </summary>
        [Category("Targets"), Description("Products to activate")]
        public string[] Products
        {
            get => products;
            set
            {
                products = value;
                if (manager != null)
                    manager.OnUpdate(LicenseField.License, this);
            }
        }

        private string[] productsApplied = Array.Empty<string>();
        /// <summary>
        /// Products applied to user when licence is redeemed
        /// </summary>
        [Category("Targets"), Description("Products applied to this user when the license was redeemed.")]
        public string[] ProductsApplied
        {
            get => productsApplied;
            set
            {
                productsApplied = value;
                if (manager != null)
                    manager.OnUpdate(LicenseField.License, this);
            }
        }

        private AccountPermission[] permissions = Array.Empty<AccountPermission>();
        /// <summary>
        /// Accounts to grant the accont on activation.
        /// </summary>
        [Category("Targets"), Description("Permissions to grant")]
        public AccountPermission[] Permissions
        {
            get => permissions;
            set
            {
                permissions = value;
                if (manager != null)
                    manager.OnUpdate(LicenseField.License, this);
            }
        }

        private AccountPermission[] permissionsApplied = Array.Empty<AccountPermission>();
        /// <summary>
        /// Permissions applied to user when licence is redeemed
        /// </summary>
        [Category("Targets"), Description("Permissions applied to this user when the license was redeemed.")]
        public AccountPermission[] PermissionsApplied
        {
            get => permissionsApplied;
            set
            {
                permissionsApplied = value;
                if (manager != null)
                    manager.OnUpdate(LicenseField.License, this);
            }
        }

        private long activateByTimestamp = 0;
        /// <summary>
        /// Timestamp when the license key expires (milliseconds, UTC unix epoch)
        /// </summary>
        [Category("Metadata"), Description("Timestamp when the license key cannot be redeemed anymore. Milliseconds since UTC Unix Epoch")]
        public long ActivateByTimestamp
        {
            get => activateByTimestamp;
            set
            {
                activateByTimestamp = value;
                if (manager != null)
                    manager.OnUpdate(LicenseField.License, this);
            }
        }

        private long createdTimestamp = 0;
        /// <summary>
        /// Timestamp when the license was created at (milliseconds, UTC unix epoch)
        /// </summary>
        [Category("Metadata"), Description("Timestamp when the license key was created. Milliseconds since UTC Unix Epoch")]
        public long CreatedTimestamp
        {
            get => createdTimestamp;
            set
            {
                createdTimestamp = value;
                if (manager != null)
                    manager.OnUpdate(LicenseField.License, this);
            }
        }

        private string createdBy = "";
        /// <summary>
        /// Username of the account that created this license key
        /// </summary>
        [Category("Metadata"), Description("Username of the account that created this license key.")]
        public string CreatedBy
        {
            get => createdBy;
            set
            {
                createdBy = value;
                if (manager != null)
                    manager.OnUpdate(LicenseField.License, this);
            }
        }

        private string groupId = "";
        [Category("Metadata"), Description("Parent Group ID")]
        public string GroupId
        {
            get => groupId;
            set
            {
                groupId = value;
                if (manager != null)
                    manager.OnUpdate(LicenseField.License, this);
            }
        }
    }
}
