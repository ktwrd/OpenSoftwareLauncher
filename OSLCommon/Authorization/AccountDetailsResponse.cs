using System;

namespace OSLCommon.Authorization
{
    public class AccountDetailsResponse
    {
        public AccountDetailsResponse()
        {
            Username = "";
            Enabled = true;
            ServiceAccount = false;
            Permissions = Array.Empty<AccountPermission>();
            DisableReasons = Array.Empty<AccountDisableReason>();
            Licenses = Array.Empty<string>();
            Groups = Array.Empty<string>();
            FirstSeenTimestamp = 0;
            LastSeenTimestamp = 0;
            Tokens = Array.Empty<AccountTokenDetailsResponse>();
        }
        public string Username { get; set; }
        public bool Enabled { get; set; }
        public bool ServiceAccount { get; set; }
        public AccountPermission[] Permissions { get; set; }
        public AccountDisableReason[] DisableReasons { get; set; }
        public string[] Licenses { get; set; }
        public string[] Groups { get; set; }
        public long FirstSeenTimestamp { get; set; }
        public long LastSeenTimestamp { get; set; }
        public AccountTokenDetailsResponse[] Tokens { get; set; }
    }
}
