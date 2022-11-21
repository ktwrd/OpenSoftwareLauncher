using kate.shared.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using OSLCommon.Logging;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;

namespace OpenSoftwareLauncher.Server
{
    public static class ElasticAdapter
    {
        public static ElasticsearchClient ElasticClient => MainClass.ElasticClient;
        public static void Initialize()
        {
            var start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var taskList = new List<Task>();

            var indexList = new List<string>
            {
                "authorizedrequest"
            };

            var enumList = GeneralHelper.GetEnumList<OSLCommon.Logging.AuditType>();
            enumList.Remove(AuditType.Any);
            foreach (var item in enumList)
                indexList.Add($"auditlog-{item}".ToLower());

            foreach (var item in indexList)
            {
                taskList.Add(new Task(delegate
                {
                    string indexName = item.ToString().ToLower();
                    var exists = ElasticClient?.Indices.ExistsAsync(indexName).Result;
                    if (!exists.Exists)
                    {
                        var res = ElasticClient?.Indices.Create(indexName);
                        if (!res.IsSuccess())
                        {
                            Console.WriteLine(res.ElasticsearchServerError.ToString());
                        }
                    }
                }));
            }

            foreach (var i in taskList)
                i.Start();
            Task.WhenAll(taskList).Wait();
            Console.WriteLine($"[ElasticAdapter.Initialize] Took {DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - start}ms for {indexList.Count} indexes");
        }
        public static void Create()
        {

            if (ServerConfig.GetBoolean("ElasticSearch", "Enable", false))
            {
                if (ServerConfig.GetBoolean("ElasticSearch", "IsCloud", false))
                {
                    MainClass.ElasticClient = new ElasticsearchClient(
                        ServerConfig.GetString("ElasticCloud", "CloudId"),
                        new ApiKey(
                            ServerConfig.GetString("ElasticSearch", "APIKey", "")));
                    Console.WriteLine("[ElasticClient] Using Cloud instance");
                    Initialize();
                }
                else
                {
                    var uriList = new List<Uri>();
                    foreach (var item in ServerConfig.ElasticSearch_URL)
                        uriList.Add(new Uri(item));
                    if (uriList.Count < 1)
                    {
                        Console.WriteLine($"[ElasticClient] No URLs supplied. Aborting");
                        Environment.Exit(1);
                    }
                    var pool = new StaticNodePool(uriList);
                    Console.WriteLine("[ElasticClient] Using URL Pool");
                    var settings = new ElasticsearchClientSettings(pool)
                        .CertificateFingerprint(ServerConfig.GetString("ElasticSearch", "Fingerprint"));
                    if (ServerConfig.GetBoolean("ElasticCloud", "BasicAuth_Enable", false))
                    {
                        Console.WriteLine("[ElasticClient] Using Basic Auth");
                        var basicAuth = new BasicAuthentication(
                            ServerConfig.GetString("ElasticCloud", "BasicAuth_Username", ""),
                            ServerConfig.GetString("ElasticCloud", "BasicAuth_Password"));
                        settings.Authentication(basicAuth);
                    }
                    else
                    {
                        Console.WriteLine("[ElasticClient] Using API Key");
                        settings.Authentication(new ApiKey(ServerConfig.GetString("ElasticSearch", "APIKey", "")));
                    }

                    MainClass.ElasticClient = new ElasticsearchClient(
                        settings);
                    Initialize();
                }
            }
            else
            {
                Console.WriteLine("[ElasticClient] Disabled");
            }
        }
    }
}
