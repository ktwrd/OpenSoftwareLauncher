using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using OSLCommon;
using OSLCommon.AutoUpdater;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace OpenSoftwareLauncher.Server
{
    public class MongoMiddle
    {
        public MongoMiddle(IServiceProvider provider)
        {
            _provider = provider;
        }
        private readonly IServiceProvider _provider;

        public string Collection_ReleaseInfo => MainClass.Config.MongoDB.Collection_ReleaseInfo;
        public string Collection_Published => MainClass.Config.MongoDB.Collection_Published;
        
        public string DatabaseName => MainClass.Config.MongoDB.DatabaseName;
        #region MongoDB Boilerplate
        public IMongoDatabase? GetDatabase()
        {
            return _provider.GetService<MongoClient>()?.GetDatabase(DatabaseName);
        }
        public IMongoCollection<ReleaseInfo>? GetReleaseCollection()
        {
            var collection = GetDatabase()?.GetCollection<ReleaseInfo>(Collection_ReleaseInfo);
            return collection;
        }
        public IMongoCollection<PublishedRelease>? GetPublishedCollection()
        {
            var collection = GetDatabase()?.GetCollection<PublishedRelease>(Collection_Published);
            return collection;
        }
        #endregion

        #region Shortcut code
        public string[] GetAllProductIds()
        {
            var collection = GetPublishedCollection();
            var filter = Builders<PublishedRelease>
                .Filter
                .Empty;
            return collection
                .Find(filter)
                .ToList()
                .Where(v => v.Release.appID.Length > 0)
                .Select(v => v.Release.appID)
                .Distinct()
                .ToArray();
        }
        public Dictionary<string, PublishedRelease> GetAllPublished()
        {
            var collection = GetPublishedCollection();
            var filter = Builders<PublishedRelease>
                .Filter
                .Empty;
            return collection
                .Find(filter)
                .ToList()
                .ToDictionary(
                    v => v.CommitHash,
                    t => t);
        }


        public void SetReleaseInfoContent(ReleaseInfo[] items)
        {
            var uidList = new List<string>();

            var collection = GetReleaseCollection();
            foreach (ReleaseInfo item in items)
            {
                var filterCount = Builders<ReleaseInfo>
                    .Filter
                    .Eq("UID", item.UID);
                var count = collection.Find(filterCount).ToList().Count;
                if (count < 1)
                    collection?.InsertOne(item);
                else
                    collection.FindOneAndReplace(filterCount, item);
                uidList.Add(item.UID);
            }

            var removeFilter = Builders<ReleaseInfo>
                .Filter
                .Nin("UID", uidList);
            collection?.DeleteMany(removeFilter);
        }
        public ReleaseInfo[] GetReleaseInfoContent()
        {
            var collection = GetReleaseCollection();

            var filter = Builders<ReleaseInfo>
                .Filter
                .Empty;

            var res = collection.Find(filter).ToList();
            return res.ToArray();
        }

        public PublishedRelease? GetPublishedReleaseByHash(string hash)
        {
            var collection = GetPublishedCollection();

            var filter = Builders<PublishedRelease>
                .Filter
                .Eq("CommitHash", hash);

            return collection.Find(filter).FirstOrDefault();
        }
        public void SetPublishedRelease(PublishedRelease content)
        {
            var collection = GetPublishedCollection();

            var filter = Builders<PublishedRelease>
                .Filter
                .Eq("UID", content.UID);

            if (collection.Find(filter).ToList().Count < 1)
                collection?.InsertOne(content);
            else
                collection?.FindOneAndReplace(filter, content);
        }
        public void ForceSetPublishedContent(PublishedRelease[] items)
        {
            var uidList = new List<string>();
            foreach (var i in items)
            {
                uidList.Add(i.UID);
                SetPublishedRelease(i);
            }

            var collection = GetPublishedCollection();
            var removeFilter = Builders<PublishedRelease>
                .Filter
                .Nin("UID", uidList);
            collection?.DeleteMany(removeFilter);
        }
        public PublishedReleaseFile[] GetPublishedFilesByHash(string hash)
        {
            var published = GetPublishedReleaseByHash(hash);
            return published?.Files ?? Array.Empty<PublishedReleaseFile>();
        }
        public void SetPublishedFilesByHash(string hash, PublishedReleaseFile[] files)
        {
            var published = GetPublishedReleaseByHash(hash);
            if (published == null)
                return;
            published.Files = files;
            SetPublishedRelease(published);
        }
        public void AddPublishedFilesByHash(string hash, PublishedReleaseFile[] files)
        {
            var published = GetPublishedReleaseByHash(hash);
            if (published == null)
                return;

            published.Files = published.Files.Concat(files).ToArray();
            SetPublishedRelease(published);
        }
        #endregion
    }
}
