using OSLCommon.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSLCommon.Licensing
{
    public class LicenseKeyMetadata
    {
        public string UID { get; set; }
        /// <summary>
        /// Allow license key to be activated
        /// </summary>
        public bool Enable { get; set; } = true;
        /// <summary>
        /// Has the license key been activated?
        /// </summary>
        public bool Activated { get; set; } = false;
        /// <summary>
        /// Username of the account that activated this license key
        /// </summary>
        public string ActivatedBy { get; set; } = "";
        /// <summary>
        /// Timestamp when the license key was activated (milliseconds, UTC unix epoch)
        /// </summary>
        public long ActivateTimestamp { get; set; } = 0;
        /// <summary>
        /// Internal note for admins
        /// </summary>
        public string InternalNote { get; set; } = "";
        /// <summary>
        /// Actual License Key
        /// </summary>
        public string Key { get; set; } = "";
        /// <summary>
        /// Products to activate
        /// </summary>
        public string[] Products { get; set; } = Array.Empty<string>();
        /// <summary>
        /// Products applied to user when licence is redeemed
        /// </summary>
        public string[] ProductsApplied { get; set; } = Array.Empty<string>();
        /// <summary>
        /// Accounts to grant the accont on activation.
        /// </summary>
        public AccountPermission[] Permissions { get; set; } = Array.Empty<AccountPermission>();
        /// <summary>
        /// Permissions applied to user when licence is redeemed
        /// </summary>
        public AccountPermission[] PermissionsApplied { get; set; } = Array.Empty<AccountPermission>();
        /// <summary>
        /// Timestamp when the license key expires (milliseconds, UTC unix epoch)
        /// </summary>
        public long ActivateByTimestamp { get; set; } = 0;
        /// <summary>
        /// Timestamp when the license was created at (milliseconds, UTC unix epoch)
        /// </summary>
        public long CreatedTimestamp { get; set; } = 0;
        /// <summary>
        /// Username of the account that created this license key
        /// </summary>
        public string CreatedBy { get; set; } = "";
        public string GroupId { get; set; }
    }
}
