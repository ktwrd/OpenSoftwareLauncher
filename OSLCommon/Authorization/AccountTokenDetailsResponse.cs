namespace OSLCommon.Authorization
{

    public class AccountTokenDetailsResponse
    {
        public string Username { get; set; }
        public bool Enabled { get; set; }
        public long CreatedTimestamp { get; set; }
        public long LastUsed { get; set; }
        public string UserAgent { get; set; }
        public string Host { get; set; }
        public string Hash { get; set; }
    }
}
