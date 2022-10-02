using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace OSLCommon.AutoUpdater
{
    public class ProductItem
    {
        public string Name = "Unknown Product";
        public string Version = "(unknown)";
        public DateTimeOffset Expiry = DateTimeOffset.FromUnixTimeMilliseconds(0);
        public DateTimeOffset UpdatedAt = DateTimeOffset.FromUnixTimeMilliseconds(0);
        public long UpdatedTimestamp = 0;
        public bool Installed = false;
        public bool Available = false;
        public bool Installing = false;
        public float InstallingPercentage = 1;

        public string LocalDirectoryName = @"product";
        public string LocalPath = @"";
        public string UpdateServerPath = @"";

        public ProductSettings Settings = new ProductSettings();
        public ProductExecutable Executable = null;

        public ProductReleaseStream TargetStream = null;
        public ReleaseInfo ReleaseInfo = null;

        public static string ToJSON(ProductItem product)
        {
            var options = new JsonSerializerOptions()
            {
                IncludeFields = true,
                IgnoreReadOnlyFields = false,
                IgnoreReadOnlyProperties = false
            };
            var output = JsonSerializer.Serialize(product, options);
            return output;
        }
        public static ProductItem FromJSON(string content)
        {
            var options = new JsonSerializerOptions()
            {
                IncludeFields = true,
                IgnoreReadOnlyFields = false,
                IgnoreReadOnlyProperties = false
            };
            var output = JsonSerializer.Deserialize<ProductItem>(content, options);
            return output;
        }
    }
}
