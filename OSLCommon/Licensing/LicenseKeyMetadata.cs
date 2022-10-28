using OSLCommon.Authorization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace OSLCommon.Licensing
{
    public class LicenseKeyMetadata
    {
        [Category("Metadata"), Description("Unique Identifier of the License Key")]
        public string UID { get; set; }

        /// <summary>
        /// Allow license key to be activated
        /// </summary>
        [Category("Metadata"), Description("Allow license key to be activated")]
        public bool Enable { get; set; } = true;
        /// <summary>
        /// Has the license key been activated?
        /// </summary>
        [Category("Metadata"), Description("Has the license key been activated")]
        public bool Activated { get; set; } = false;
        /// <summary>
        /// Username of the account that activated this license key
        /// </summary>
        [Category("Activator"), Description("Username of the account that activated this license key.")]
        public string ActivatedBy { get; set; } = "";
        /// <summary>
        /// Timestamp when the license key was activated (milliseconds, UTC unix epoch)
        /// </summary>
        [Category("Activator"), Description("Timestamp when the license key was activated. Milliseconds since UTC Unix Epoch")]
        public long ActivateTimestamp { get; set; } = 0;
        /// <summary>
        /// Internal note for admins
        /// </summary>
        [Category("Metadata")]
        public string InternalNote { get; set; } = "";
        /// <summary>
        /// Actual License Key
        /// </summary>
        [Category("Metadata"), Description("License Key")]
        public string Key { get; set; } = "";
        /// <summary>
        /// Products to activate
        /// </summary>
        [Category("Targets"), Description("Products to activate")]
        public string[] Products { get; set; } = Array.Empty<string>();
        /// <summary>
        /// Products applied to user when licence is redeemed
        /// </summary>
        [Category("Targets"), Description("Products applied to this user when the license was redeemed.")]
        public string[] ProductsApplied { get; set; } = Array.Empty<string>();
        /// <summary>
        /// Accounts to grant the accont on activation.
        /// </summary>
        [Category("Targets"), Description("Permissions to grant")]
        public AccountPermission[] Permissions { get; set; } = Array.Empty<AccountPermission>();
        /// <summary>
        /// Permissions applied to user when licence is redeemed
        /// </summary>
        [Category("Targets"), Description("Permissions applied to this user when the license was redeemed.")]
        public AccountPermission[] PermissionsApplied { get; set; } = Array.Empty<AccountPermission>();
        /// <summary>
        /// Timestamp when the license key expires (milliseconds, UTC unix epoch)
        /// </summary>
        [Category("Metadata"), Description("Timestamp when the license key cannot be redeemed anymore. Milliseconds since UTC Unix Epoch")]
        public long ActivateByTimestamp { get; set; } = 0;
        /// <summary>
        /// Timestamp when the license was created at (milliseconds, UTC unix epoch)
        /// </summary>
        [Category("Metadata"), Description("Timestamp when the license key was created. Milliseconds since UTC Unix Epoch")]
        public long CreatedTimestamp { get; set; } = 0;
        /// <summary>
        /// Username of the account that created this license key
        /// </summary>
        [Category("Metadata"), Description("Username of the account that created this license key.")]
        public string CreatedBy { get; set; } = "";
        [Category("Metadata"), Description("Parent Group ID")]
        public string GroupId { get; set; }
    }
}
