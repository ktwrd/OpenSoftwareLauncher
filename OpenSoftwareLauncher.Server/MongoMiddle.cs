using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using OSLCommon;
using OSLCommon.AutoUpdater;

namespace OpenSoftwareLauncher.Server
{
    public class MongoMiddle
    {
        public ServiceProvider Provider;

        public string Collection_ReleaseInfo => ServerConfig.GetString("MongoDB", "Collection_ReleaseInfo");
        public string Collection_Published => ServerConfig.GetString("MongoDB", "Collection_Published");

        public string DatabaseName => ServerConfig.GetString("MongoDB", "DatabaseName");
        public IMongoDatabase? GetDatabase()
        {
            var db = Provider.GetService<MongoClient>()?.GetDatabase(DatabaseName);
            return db;
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
