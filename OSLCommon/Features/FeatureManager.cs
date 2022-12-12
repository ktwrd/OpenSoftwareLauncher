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

        public bool ContainsURL(string url)
        {
            var filter = Builders<Feature>
                .Filter
                .Eq("URL", url);

            return GetFeatureCollection<Feature>().Find(filter).CountDocuments() > 0;
        }
        public bool ContainsName(string name)
        {
            var filter = Builders<Feature>
                .Filter
                .Eq("Name", name);

            return GetFeatureCollection<Feature>().Find(filter).CountDocuments() > 0;
        }

        /// <returns>Success</returns>
        public bool Create(string name, string url)
        {
            if (ContainsName(name))
                return false;
            if (ContainsURL(url))
                return false;

            var instance = new Feature()
            {
                Name = name,
                URL = url
            };

            var collection = GetFeatureCollection<Feature>();
            collection.InsertOne(instance);
            return true;
        }

        public bool DeleteByName(string name)
        {
            if (!ContainsName(name))
                return false;

            var filter = Builders<Feature>
                .Filter
                .Eq("Name", name);

            GetFeatureCollection<Feature>().DeleteOne(filter);
            return true;
        }
        public bool DeleteByURL(string url)
        {
            if (!ContainsURL(url))
                return false;

            var filter = Builders<Feature>
                .Filter
                .Eq(field: "URL", url);

            GetFeatureCollection<Feature>().DeleteOne(filter);
            return true;
        }
    }
}
