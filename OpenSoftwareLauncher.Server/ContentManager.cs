using kate.shared.Helpers;
using OpenSoftwareLauncher.Shared;
using OpenSoftwareLauncher.Shared.Distribution;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace OpenSoftwareLauncher.Server
{
    public class ContentManagerSummary
    {
        public ContentManagerSummary()
        {
            AppBranches = new List<AppBranch>();
            Depots = new Dictionary<string, Depot>();
            DepotFiles = new Dictionary<string, DepotFile>();
            Packages = new Dictionary<string, AppPackage>();
        }
        public List<AppBranch> AppBranches;
        public Dictionary<string, Depot> Depots;
        public Dictionary<string, DepotFile> DepotFiles;
        public Dictionary<string, AppPackage> Packages;
    }
    public class ContentManager : ContentManagerSummary
    {
        public ContentManager() : base()
        {
            DeserializeContent();
        }

        private void DeserializeContent()
        {
            if (!File.Exists(Path.Combine(Program.DataDirectory, @"content.json"))) return;
            var fileContent = File.ReadAllText(Path.Combine(Program.DataDirectory, @"content.json"));
            var deserialized = JsonSerializer.Deserialize<ContentManagerSummary>(fileContent, Program.serializerOptions);
            if (deserialized == null)
            {
                Console.WriteLine($"[OpenSoftwareLauncher.Server.ContentManager] [ERR] Content was deserliazed as null.");
                return;
            }
            AppBranches = deserialized.AppBranches;
            Depots = deserialized.Depots;
            DepotFiles = deserialized.DepotFiles;
            Packages = deserialized.Packages;

            foreach (var depot in Depots)
            {
                var files = new List<DepotFile>();
                foreach (var hash in depot.Value.FileHashes)
                {
                    if (DepotFiles.ContainsKey(hash))
                        files.Add(DepotFiles[hash]);
                }
                Depots[depot.Key].Files = files.ToArray();
            }
        }

        public bool PendingWrite { get; private set; }

        public void DeleteDepotFile(string depotFileId)
        {
            if (DepotFiles.ContainsKey(depotFileId))
            {
                DepotFiles.Remove(depotFileId);
                foreach (var depot in Depots)
                {
                    if (depot.Value.FileHashes.Contains(depotFileId))
                    {
                        depot.Value.FileHashes = depot.Value.FileHashes.Where(v => v != depotFileId).ToArray();
                        depot.Value.Files = depot.Value.Files.Where(v => v.Hash != depotFileId).ToArray();
                    }
                }
                DepotFileRemoved?.Invoke(depotFileId);
                PendingWrite = true;
            }
        }

        public void DeleteDepot(string depotId)
        {
            if (!Depots.ContainsKey(depotId))
            {
                throw new Exception($"Depot {depotId} does not exist");
            }
            var target = Depots[depotId];

            // Get list of files referenced
            var fileHashes = target.FileHashes;
            var fileHashReferenceCount = new Dictionary<string, int>();
            foreach (var file in DepotFiles)
            {
                if (!fileHashes.Contains(file.Key))
                    continue;
                if (!fileHashReferenceCount.ContainsKey(file.Key))
                    fileHashReferenceCount.Add(file.Key, 0);
                fileHashReferenceCount[file.Key]++;
            }

            Depots.Remove(depotId);
            PendingWrite = true;
            DepotRemoved?.Invoke(depotId);

            foreach (var referenceCount in fileHashReferenceCount)
            {
                if (referenceCount.Value < 2)
                {
                    DeleteDepotFile(referenceCount.Key);
                }
            }
        }

        public event StringDelegate DepotRemoved;
        public event StringDelegate DepotFileRemoved;
    }
}
