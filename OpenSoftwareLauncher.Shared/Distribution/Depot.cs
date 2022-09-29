using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenSoftwareLauncher.Shared.Distribution
{
    public class DepotFile
    {
        /// <summary>
        /// Relative location of file from depot "root"
        /// </summary>
        public string RelativeFilePath { get; set; } = "";
        /// <summary>
        /// Calculated with SHA256
        /// </summary>
        public string Hash { get; set; } = "";
        /// <summary>
        /// Seconds since File First Created, Unix Epoch in UTC Timezone.
        /// </summary>
        public long Timestamp { get; set; } = 0;
        /// <summary>
        /// File size in bytes
        /// </summary>
        public long Size { get; set; } = 0;
    }
    public class Depot
    {
        public string DepotId { get; set; } = "";
        [JsonIgnore]
        public DepotFile[] Files { get; set; } = Array.Empty<DepotFile>();
        public string[] FileHashes { get; set; } = Array.Empty<string>();
        public long Timestamp { get; set; } = 0;
        /// <summary>
        /// SHA256 Hash of all <see cref="Depot.Files"/> Hashes.
        /// </summary>
        public string Hash { get; set; } = "";

        public eOperatingSystem OperatingSystem { get; set; } = eOperatingSystem.Any;
        public eArchitecture Architecture { get; set; } = eArchitecture.Any;
    }
}
