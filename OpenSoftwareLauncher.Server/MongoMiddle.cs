using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using OSLCommon;
using OSLCommon.AutoUpdater;
using System;
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

        public string Collection_ReleaseInfo => ServerConfig.GetString("MongoDB", "Collection_ReleaseInfo");
        public string Collection_Published => ServerConfig.GetString("MongoDB", "Collection_Published");

        public string DatabaseName => ServerConfig.GetString("MongoDB", "DatabaseName");
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
    }
}
