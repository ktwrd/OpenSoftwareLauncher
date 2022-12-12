using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSLCommon.Features
{
    public class FeatureManager
    {
        internal MongoClient mongoClient;
        public FeatureManager(MongoClient client)
        {
            mongoClient = client;
        }

        public string DatabaseName = "opensoftwarelauncher";
        public string CollectionName = "features";
        private IMongoCollection<T> GetFeatureCollection<T>()
        {
            return mongoClient.GetDatabase(DatabaseName).GetCollection<T>(CollectionName);
        }

        public Feature[] GetAll()
        {
            var filter = Builders<Feature>
                .Filter
                .Empty;

            var exec = GetFeatureCollection<Feature>()
                .Find(filter)
                .ToList();

            return exec.ToArray();
        }
    }
}
