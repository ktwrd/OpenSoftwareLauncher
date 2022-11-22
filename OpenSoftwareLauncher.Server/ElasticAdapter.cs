﻿using kate.shared.Helpers;
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
        public static ElasticsearchClient? Client;
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
                    var exists = Client?.Indices.ExistsAsync(indexName).Result;
                    if (!exists.Exists)
                    {
                        var res = Client?.Indices.Create(indexName);
                        if (res.IsSuccess())
                        {
                            Console.WriteLine($"[ElasticAdapter.Initialize] Create index \"{item}\"");
                        }
                        else
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
                    Client = new ElasticsearchClient(
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

                    Client = new ElasticsearchClient(
                        settings);
                    Initialize();
                }
            }
            else
            {
                Console.WriteLine("[ElasticClient] Disabled");
        public static string IndexPrefix
        {
            get
            {
                return ServerConfig.GetString("ElasticSearch", "IndexPrefix", "osl-");
            }
            set
            {
                ServerConfig.Set("ElasticSearch", "IndexPrefix", value);
            }
        }

        public static Task ASPMiddleware(HttpContext context, Func<Task> next)
        {
            var possibleAddress = context.Connection.RemoteIpAddress?.ToString() ?? "";
            if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
                possibleAddress = context.Request.Headers["X-Forwarded-For"];
            else if (context.Request.Headers.ContainsKey("X-Real-IP"))
                possibleAddress = context.Request.Headers["X-Real-IP"];

            if (context.Request.Query.ContainsKey("token"))
            {
                var account = MainClass.contentManager.AccountManager.GetAccount(context.Request.Query["token"], bumpLastUsed: true);
                if (account != null)
                {
                    string indexName = IndexPrefix + "authorizedrequest";
                    var res = Client?.Index(new AuthorizedRequestEntry()
                    {
                        Username = account.Username,
                        Path = context.Request.Path,
                        UserAgent = context.Request.Headers.UserAgent,
                        Address = possibleAddress,
                        Method = context.Request.Method
                    }, request => request.Index(indexName));
                    if (!res.IsSuccess())
                    {
                        CPrint.Error($"[ElasticAdapter.ASPMiddleware] Failed to index in \"{indexName}\"\n{res.ElasticsearchServerError.Error}");
                    }
                }
            }

            return next();
        }
            }
        }
    }
}
