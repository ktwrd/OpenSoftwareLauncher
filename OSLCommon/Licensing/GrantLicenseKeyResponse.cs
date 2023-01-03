using OSLCommon.Authorization;

namespace OSLCommon.Licensing
{
    public enum GrantLicenseKeyResponseCode
    {
        Invalid,
        AlreadyRedeemed,
        Granted,
        Details,
        Available
    }
    public class GrantLicenseKeyResponse
    {
        public GrantLicenseKeyResponseCode Code { get; set; }
        /// <summary>
        /// Products Granted
        /// </summary>
        public string[] Products { get; set; }
        /// <summary>
        /// Permissions Granted
        /// </summary>
        public AccountPermission[] Permissions { get; set; }
    }
}
