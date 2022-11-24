
namespace OSLCommon.Authorization
{
    public class GrantTokenResponse
    {
        public GrantTokenResponse(string message = "", bool success = false, AccountToken token = null, string[] groups=null, string[] licenses = null, AccountPermission[] permissions = null)
        {
            Message = message;
            Token = token;
            Success = success;
            Groups = groups == null ? new string[] { } : groups;
            Licenses = licenses == null ? new string[] { } : licenses;
            Permissions = permissions == null ? new AccountPermission[] { } : permissions;
        }
        public bool Success { get; set; }
        public string Message { get; set; }

        public string[] Groups { get; set; }
        public string[] Licenses { get; set; }
        public AccountPermission[] Permissions { get; set; }

        /// <summary>
        /// Nullable
        /// </summary>
        public AccountToken Token { get; set; }
    }
}
