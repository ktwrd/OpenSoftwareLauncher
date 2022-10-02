using OSLCommon.AutoUpdater;
using OSLCommon;
using Google.Cloud.Firestore;
using kate.shared.Helpers;
using System.Text.Json;
using System.Collections.Generic;
using System.IO;
using System;
using System.Diagnostics;
using OSLCommon.Authorization;
using OpenSoftwareLauncher.Server.OpenSoftwareLauncher.Server;

namespace OpenSoftwareLauncher.Server
{
    public class ContentManager
    {
        public List<ReleaseInfo> ReleaseInfoContent = new();
        public Dictionary<string, ProductRelease> Releases = new();
        public Dictionary<string, PublishedRelease> Published = new();
        public AccountManager AccountManager = new();
        public SystemAnnouncement SystemAnnouncement = new();

        /*internal List<string> LoadedFirebaseAssets = new();
         * internal FirestoreDb database;*/
         
        public ContentManager()
        {
            databaseDeserialize();

            AccountManager.PendingWrite += AccountManager_PendingWrite;
            SystemAnnouncement.Update += SystemAnnouncement_Update;
        }
        

        private void SystemAnnouncement_Update()
        {
            File.WriteAllText(JSON_SYSANNOUNCE_FILENAME, SystemAnnouncement.ToJSON());
            string txt = $"[ContentManager->SystemAnnouncement_Update]  {Path.GetRelativePath(Directory.GetCurrentDirectory(), JSON_SYSANNOUNCE_FILENAME)}";
            Trace.WriteLine(txt);
            Console.WriteLine(txt);
            ServerConfig.Save();
        }

        private void AccountManager_PendingWrite()
        {
            File.WriteAllText(JSON_ACCOUNT_FILENAME, AccountManager.ToJSON());
            AccountManager.ClearPendingWrite();
            string txt = $"[ContentManager->AccountManager_PendingWrite] {Path.GetRelativePath(Directory.GetCurrentDirectory(), JSON_ACCOUNT_FILENAME)}";
            Trace.WriteLine(txt);
            Console.WriteLine(txt);
            ServerConfig.Save();
        }

        /*private readonly string DATABASE_FILENAME = Path.Combine(
            MainClass.DataDirectory,
            "content.db");*/
        private readonly string JSONBACKUP_FILENAME = Path.Combine(
            MainClass.DataDirectory,
            "content.json");
        private readonly string JSON_ACCOUNT_FILENAME = Path.Combine(
            MainClass.DataDirectory,
            "account.json");
        private readonly string JSON_SYSANNOUNCE_FILENAME = Path.Combine(
            MainClass.DataDirectory,
            "systemAnnouncement.json");
        private int DatabaseVersion;
        private class JSONBackupContent
        {
            public List<ReleaseInfo> ReleaseInfoContent = new();
            public Dictionary<string, ProductRelease> Releases = new();
            public Dictionary<string, PublishedRelease> Published = new();
        }
        private void databaseDeserialize()
        {
            try
            {
                if (File.Exists(JSON_ACCOUNT_FILENAME))
                    AccountManager.Read(File.ReadAllText(JSON_ACCOUNT_FILENAME));
            }
            catch (Exception except)
            {
                string txt = $"[ContentManager->databaseSerialize:{GeneralHelper.GetNanoseconds()}] [ERR] Failed to read Account Details\n--------\n{except}\n--------\n";
                Trace.WriteLine(txt);
                Console.Error.WriteLine(txt);
            }
            try
            {
                if (File.Exists(JSON_SYSANNOUNCE_FILENAME))
                    SystemAnnouncement.Read(File.ReadAllText(JSON_SYSANNOUNCE_FILENAME));
            }
            catch (Exception except)
            {
                string txt = $"[ContentManager->databaseSerialize:{GeneralHelper.GetNanoseconds()}] [ERR] Failed to read Announcement Details\n--------\n{except}\n--------\n";
                Trace.WriteLine(txt);
                Console.Error.WriteLine(txt);
            }
            if (File.Exists(JSONBACKUP_FILENAME))
            {
                RestoreFromJSON();
            }
            /*DatabaseHelper.Read(DATABASE_FILENAME, sr =>
            {
                DatabaseVersion = sr.ReadInt32(); // ContainsKey
                ReleaseInfoContent = (List<ReleaseInfo>)sr.ReadBList<ReleaseInfo>();
                Releases = (Dictionary<string, ProductRelease>)sr.ReadDictionary<string, ProductRelease>();
                Published = (Dictionary<string, PublishedRelease>)sr.ReadDictionary<string, PublishedRelease>();
                Console.WriteLine($"[ContentManager->databaseDeserialize] Read {Path.GetRelativePath(Directory.GetCurrentDirectory(), DATABASE_FILENAME)}");
            }, (e) =>
            {
                Console.WriteLine(@"//-- content.db is corrupt...".PadLeft(Console.BufferWidth));
                Console.Error.WriteLine(e);
                Console.WriteLine(@"//-- content.db is corrupt...".PadLeft(Console.BufferWidth));
                File.Copy("content.db", $"content.{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.bak.db");
                if (File.Exists(JSONBACKUP_FILENAME))
                    RestoreFromJSON();
            });*/
        }
        private void RestoreFromJSON()
        {
            JSONBackupContent? deserialized = null;
            Exception? deserializeException = null;
            try
            {
                deserialized = JsonSerializer.Deserialize<JSONBackupContent>(File.ReadAllText(JSONBACKUP_FILENAME), MainClass.serializerOptions);
            }
            catch
            (Exception e)
            {
                deserializeException = e;
            }
            if (deserialized == null)
            {
                string addedContent = "";
                if (deserializeException != null)
                    addedContent = "\n" + deserializeException?.ToString() + "\n";
                Console.Error.Write($"\n[ContentManager->RestoreFromJSON] Failed to restore ;w;{addedContent}");
                return;
            }
            Console.WriteLine($"[ContentManager->RestoreFromJSON] Restored from JSON.");

            ReleaseInfoContent = deserialized.ReleaseInfoContent;
            Releases = ReleaseHelper.TransformReleaseList(ReleaseInfoContent.ToArray());
            Published = deserialized.Published;
            System.Threading.Thread.Sleep(500);
            DatabaseSerialize();
        }
        public void DatabaseSerialize()
        {
            /*bool response = DatabaseHelper.Write(DATABASE_FILENAME, sw =>
            {
                sw.Write(DatabaseVersion);
                sw.Write(ReleaseInfoContent);
                sw.Write(Releases);
                sw.Write(Published);
            });
            if (response)
            {
                Console.WriteLine($"[ContentManager->DatabaseSerialize] Saved {Path.GetRelativePath(Directory.GetCurrentDirectory(), DATABASE_FILENAME)}");
                CreateJSONBackup();
            }
            else
            {
                Console.Error.WriteLine($"[ContentManager->DatabaseSerialize] Failed to save {Path.GetRelativePath(Directory.GetCurrentDirectory(), DATABASE_FILENAME)}");
            }*/
            SystemAnnouncement.OnUpdate();
            AccountManager.ForcePendingWrite();
            CreateJSONBackup();
            ServerConfig.Save();
        }
        private void CreateJSONBackup()
        {
            var data = new Dictionary<string, object>()
            {
                {"ReleaseInfoContent", ReleaseInfoContent },
                {"Releases", Releases },
                {"Published", Published }
            };
            File.WriteAllText(JSONBACKUP_FILENAME, JsonSerializer.Serialize(data, MainClass.serializerOptions));
        }
        /*static ContentManager()
        {
            FirebaseHelper.FirebaseCollection.Add(typeof(PublishedRelease), "PublishedRelease");
            FirebaseHelper.FirebaseCollection.Add(typeof(PublishedReleaseFile), "PublishedReleaseFile");
            FirebaseHelper.FirebaseCollection.Add(typeof(ProductReleaseStream), "ProductReleaseStream");
            FirebaseHelper.FirebaseCollection.Add(typeof(ProductRelease), "ProductRelease");
            FirebaseHelper.FirebaseCollection.Add(typeof(ReleaseInfo), "Release");
            FirebaseHelper.FirebaseCollection.Add(typeof(ProductExecutable), "ProductExecutables");
        }

        private Thread firebaseSaveThread;
        private Thread firebaseLoadThread;
        private void firebase()
        {
            BusStationTimer = new System.Threading.Timer(CheckBusSchedule, new AutoResetEvent(false), 0, 150);
            TriggerSave = false;
            TriggerSaveTimestamp = -1;
            TriggerLoad = false;
            TriggerLoadTimestamp = -1;
            IsSaving = false;
            IsLoading = false;
            firebaseSaveThread = new Thread(firebaseSaveThreadLogic);
            firebaseLoadThread = new Thread(firebaseLoadThreadLogic);
            database = FirestoreDb.Create(@"cloudtesting-3d734");

            // We do this because we want to lock the process
            // before the web server starts, just to be safe.
            firebaseLoadThreadLogic();
        }

        public void DatabaseSerialize()
        {

        }

        private Timer BusStationTimer;

        #region Schedule Saving
        public bool TriggerSave { get; private set; }
        public long TriggerSaveTimestamp { get; private set; }
        public void ScheduleSave()
        {
            Console.WriteLine($"ScheduleSave: (TriggerLoad: {TriggerLoad},\n              TriggerSave: {TriggerSave},\n              TriggerLoadTimestamp: {TriggerLoadTimestamp},\n              TriggerSaveTimestamp: {TriggerSaveTimestamp},\n              IsSaving: {IsSaving},\n              IsLoading: {IsLoading})");
            if (TriggerSave || TriggerLoad || TriggerSaveTimestamp > 0) return;
            TriggerSave = true;
            TriggerSaveTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
        #endregion
        #region Schedule Loading
        public bool TriggerLoad { get; set; }
        public long TriggerLoadTimestamp { get; private set; }
        public void ScheduleLoad()
        {
            Console.WriteLine($"ScheduleSave: (TriggerLoad: {TriggerLoad},\n              TriggerSave: {TriggerSave},\n              TriggerLoadTimestamp: {TriggerLoadTimestamp},\n              TriggerSaveTimestamp: {TriggerSaveTimestamp},\n              IsSaving: {IsSaving},\n              IsLoading: {IsLoading})");
            if (TriggerLoad || TriggerSave || TriggerLoadTimestamp > 0) return;
            TriggerLoad = true;
            TriggerLoadTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
        #endregion
        #region Schedule Checker
        public bool IsSaving{ get; private set; }
        public bool IsLoading { get; private set; }
        public bool WillScheduleSave => !(TriggerSave || TriggerLoad || TriggerSaveTimestamp > 0);
        public bool WillScheduleLoad => !(TriggerLoad || TriggerSave || TriggerLoadTimestamp > 0);
        private void CheckBusSchedule(object? state)
        {
            if (state == null) return;
            AutoResetEvent autoEvent = (AutoResetEvent)state;
            if (TriggerLoadTimestamp > 0 && !IsSaving && !IsLoading)
            {
                if ((TriggerLoadTimestamp < TriggerSaveTimestamp || TriggerSaveTimestamp <= 0) && TriggerLoad)
                {
                    TriggerLoad = false;
                    LoadFirebase();
                }
            }
            if (TriggerSaveTimestamp > 0 && !IsSaving && !IsLoading)
            {
                if ((TriggerSaveTimestamp < TriggerLoadTimestamp || TriggerLoadTimestamp <= 0) && TriggerSave)
                {
                    TriggerSave = false;
                    SaveFirebase();
                }
            }
            autoEvent.Set();
        }
        #endregion

        #region Save Content from Firebase
        public void SaveFirebase()
        {
            if (firebaseSaveThread.ThreadState == ThreadState.Running) return;
            if (firebaseSaveThread.ThreadState == ThreadState.Stopped)
                firebaseSaveThread = new Thread(firebaseSaveThreadLogic);
            firebaseSaveThread.Start();
        }
        private void firebaseSaveThreadLogic()
        {
            Console.WriteLine($"[ContentManager->SaveFirebase] Uploading Content to Firebase");
            IsSaving = true;
            var startTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var count = 0;
            List<ReleaseInfo> working_ReleaseInfoContent = JsonSerializer.Deserialize<List<ReleaseInfo>>(JsonSerializer.Serialize(this.ReleaseInfoContent, MainClass.serializerOptions), MainClass.serializerOptions) ?? new List<ReleaseInfo>();
            Dictionary<string, ProductRelease> working_Releases = JsonSerializer.Deserialize<Dictionary<string, ProductRelease>>(JsonSerializer.Serialize(this.Releases, MainClass.serializerOptions), MainClass.serializerOptions) ?? new Dictionary<string, ProductRelease>();
            Dictionary<string, PublishedRelease> working_Published = JsonSerializer.Deserialize<Dictionary<string, PublishedRelease>>(JsonSerializer.Serialize(this.Published, MainClass.serializerOptions), MainClass.serializerOptions) ?? new Dictionary<string, PublishedRelease>();
            var fetchedCount = 0;
            var taskList = new List<Task>();
            VoidDelegate del = delegate
            {
                fetchedCount++;
            };
            foreach (var item in working_ReleaseInfoContent)
            {
                taskList.Add(new Task(new Action(delegate
                {
                    item.ToFirebase(item.GetFirebaseDocumentReference(database), del).Wait();
                    count++;
                })));
            }
            foreach (var pair in working_Releases)
            {
                taskList.Add(new Task(new Action(delegate
                {
                    pair.Value.ToFirebase(pair.Value.GetFirebaseDocumentReference(database), del).Wait();
                    count++;
                    count += pair.Value.Streams.Length * 2;
                })));
            }
            foreach (var pair in working_Published)
            {
                taskList.Add(new Task(new Action(delegate
                {
                    pair.Value.ToFirebase(pair.Value.GetFirebaseDocumentReference(database), del).Wait();
                    count++;
                    count += pair.Value.Files.Length + 1;
                })));
            }
            foreach (var i in taskList)
                i.Start();
            Task.WaitAll(taskList.ToArray());
            Console.WriteLine($"[ContentManager->SaveFirebase] Uploaded {count} items in {DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - startTimestamp}ms");
            working_Releases.Clear();
            working_Published.Clear();
            working_ReleaseInfoContent.Clear();
            TriggerSaveTimestamp = -1;
            IsSaving = false;
        }
        #endregion
        #region Load Content from Firebase
        public void LoadFirebase()
        {
            if (firebaseLoadThread.ThreadState == ThreadState.Running) return;
            if (firebaseLoadThread.ThreadState == ThreadState.Stopped)
                firebaseLoadThread = new Thread(firebaseLoadThreadLogic);
            firebaseLoadThread.Start();
        }
        private void firebaseLoadThreadLogic()
        {
            IsLoading = true;
            Console.WriteLine($"[ContentManager->LoadFirebase] Fetching Content from Firebase");
            var startTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var count = 0;
            var fetchedCount = 0;

            List<ReleaseInfo> working_ReleaseInfoContent = JsonSerializer.Deserialize<List<ReleaseInfo>>(JsonSerializer.Serialize(this.ReleaseInfoContent, MainClass.serializerOptions), MainClass.serializerOptions) ?? new List<ReleaseInfo>();
            Dictionary<string, ProductRelease> working_Releases = JsonSerializer.Deserialize<Dictionary<string, ProductRelease>>(JsonSerializer.Serialize(this.Releases, MainClass.serializerOptions), MainClass.serializerOptions) ?? new Dictionary<string, ProductRelease>();
            Dictionary<string, PublishedRelease> working_Published = JsonSerializer.Deserialize<Dictionary<string, PublishedRelease>>(JsonSerializer.Serialize(this.Published, MainClass.serializerOptions), MainClass.serializerOptions) ?? new Dictionary<string, PublishedRelease>();

            var taskList = new List<Task>();
            VoidDelegate del = delegate
            {
                fetchedCount++;
            };

            foreach (var item in working_ReleaseInfoContent)
            {
                taskList.Add(new Task(new Action(delegate
                {
                    var snapshot = item.GetFirebaseDocumentReference(database).GetSnapshotAsync().Result;
                    item.FromFirebase(snapshot, del).Wait();
                    count++;
                })));
            }
            foreach (var pair in working_Releases)
            {
                taskList.Add(new Task(new Action(delegate
                {
                    var snapshot = pair.Value.GetFirebaseDocumentReference(database).GetSnapshotAsync().Result;
                    pair.Value.FromFirebase(snapshot, del).Wait();
                    count++;
                    count += pair.Value.Streams.Length * 2;
                })));
            }
            foreach (var pair in working_Published)
            {
                taskList.Add(new Task(new Action(delegate
                {
                    var snapshot = pair.Value.GetFirebaseDocumentReference(database).GetSnapshotAsync().Result;
                    pair.Value.FromFirebase(snapshot, del).Wait();
                    count++;
                    count += pair.Value.Files.Length + 1;
                })));
            }

            var newReleaseInfoContent = new List<ReleaseInfo>();
            var newReleases = new Dictionary<string, ProductRelease>();
            var newPublished = new Dictionary<string, PublishedRelease>();

            var collectionListAsync = database.ListRootCollectionsAsync().ToListAsync().Result;
            foreach (var collection in collectionListAsync)
            {
                if (
                collection.Path.EndsWith(FirebaseHelper.FirebaseCollection[typeof(ReleaseInfo)])
                || collection.Path.EndsWith(FirebaseHelper.FirebaseCollection[typeof(ProductRelease)])
                || collection.Path.EndsWith(FirebaseHelper.FirebaseCollection[typeof(PublishedRelease)]))
                {
                    var documents = collection.GetSnapshotAsync().Result;
                    foreach (var doc in documents)
                    {
                        if (LoadedFirebaseAssets.Contains(doc.Reference.Path))
                            continue;

                        switch (doc.Reference.Path.Split("/documents/")[1].Split("/")[0])
                        {
                            case "Release":
                                taskList.Add(new Task(new Action(delegate
                                {
                                    var rel = new ReleaseInfo();
                                    rel.FromFirebase(doc, del).Wait();
                                    newReleaseInfoContent.Add(rel);
                                    LoadedFirebaseAssets.Add(doc.Reference.Path);
                                    count++;
                                })));
                                break;
                            case "ProductRelease":
                                taskList.Add(new Task(new Action(delegate
                                {
                                    var prodrel = new ProductRelease();
                                    prodrel.FromFirebase(doc, del).Wait();
                                    if (newReleases.ContainsKey(prodrel.ProductID))
                                        newReleases[prodrel.ProductID].Streams = new List<ProductReleaseStream>(newReleases[prodrel.ProductID].Streams.Concat(prodrel.Streams)).ToArray();
                                    LoadedFirebaseAssets.Add(doc.Reference.Path);
                                    count++;
                                    count += prodrel.Streams.Length * 2;
                                })));
                                break;
                            case "PublishedRelease":
                                taskList.Add(new Task(new Action(delegate
                                {
                                    var pubrel = new PublishedRelease();
                                    pubrel.FromFirebase(doc, del).Wait();
                                    if (pubrel != null && pubrel.CommitHash != null)
                                    {
                                        newPublished.Add(pubrel.CommitHash, pubrel);
                                        LoadedFirebaseAssets.Add(doc.Reference.Path);
                                        count++;
                                        count += pubrel.Files.Length;
                                    }
                                })));
                                break;
                        }
                    }
                }
            }

            foreach (var i in taskList)
                i.Start();
            Task.WaitAll(taskList.ToArray());

            working_Releases = new(working_Releases.Concat(newReleases));
            working_Published = new(working_Published.Concat(newPublished));
            working_ReleaseInfoContent = new(working_ReleaseInfoContent.Concat(newReleaseInfoContent));
            Console.WriteLine($"[ContentManager->LoadFirebase] Fetched {count} items in {DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - startTimestamp}ms");
            Releases = new(working_Releases);
            Published = new(working_Published);
            ReleaseInfoContent = new(working_ReleaseInfoContent);
            TriggerLoadTimestamp = -1;
            IsLoading = false;
        }
        #endregion*/
    }
}
