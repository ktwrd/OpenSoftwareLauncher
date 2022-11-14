using OSLCommon.Authorization;

namespace OSLCommon.Licensing
{
    public class CreateProductKeyRequest
    {
        public int Count { get; set; }
        public string[] RemoteLocations { get; set; }
        public AccountPermission[] Permissions { get; set; }
        public long ExpiryTimestamp { get; set; }
        public string Note { get; set; }
        public string GroupLabel { get; set; }
    }
}
