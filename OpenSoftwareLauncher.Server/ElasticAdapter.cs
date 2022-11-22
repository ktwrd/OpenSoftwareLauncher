using kate.shared.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using OSLCommon.Logging;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using System.Text.Json;
using OSLCommon;
using OSLCommon.Logging.Elastic;
using System.Linq;
using Microsoft.AspNetCore.Http;

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
                    string indexName = IndexPrefix + item.ToString().ToLower();
                    indexName = indexName.ToLower();
                    var exists = Client?.Indices.ExistsAsync(indexName).Result;
                    if (!exists.Exists)
                    {
                        var res = Client?.Indices.Create(indexName);
                        if (res.IsSuccess())
                        {
                            CPrint.Debug($"[ElasticAdapter.Initialize] Create index \"{item}\"");
                        }
                        else
                        {
                            CPrint.Error($"[ElasticAdapter.Initialize] Failed to create index \"{item}\"\n{res.ElasticsearchServerError}");
                        }
                        if (res.ElasticsearchWarnings.Count() > 0)
                            for (int i = 0; i < res.ElasticsearchWarnings.Count(); i++)
                                CPrint.Warn($"[ElasticAdapter.Initialize] ({i}) Warn while creating index \"{item}\": {res.ElasticsearchWarnings.ElementAt(i)}");
                    }
                }));
            }

            foreach (var i in taskList)
                i.Start();
            Task.WhenAll(taskList).Wait();
            CPrint.Debug($"[ElasticAdapter.Initialize] Took {DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - start}ms for {indexList.Count} indexes");

            MainClass.Ready += delegate
            {
                MainClass.contentManager.AuditLogManager.CreateEntry += AuditCreateHandle;
            };
        }


        public static void Create()
        {

            if (ServerConfig.GetBoolean("ElasticSearch", "Enable", false))
            {
                if (ServerConfig.GetBoolean("ElasticSearch", "IsCloud", false))
                {
                    string cloudId = ServerConfig.GetString("ElasticCloud", "CloudId");
                    Client = new ElasticsearchClient(
                        cloudId,
                        new ApiKey(
                            ServerConfig.GetString("ElasticSearch", "APIKey", "")));
                    CPrint.WriteLine($"[ElasticAdapter] Using Cloud Instance (ID: \"{cloudId}\")");
                    Initialize();
                }
                else
                {
                    var uriList = new List<Uri>();
                    foreach (var item in ServerConfig.ElasticSearch_URL)
                        uriList.Add(new Uri(item));
                    if (uriList.Count < 1)
                    {
                        CPrint.Error($"[ElasticAdapter] Empty array at \"ElasticSearch.URL\" in ServerConfig. Aborting");
                        Environment.Exit(1);
                    }
                    var pool = new StaticNodePool(uriList);
                    var settings = new ElasticsearchClientSettings(pool)
                        .CertificateFingerprint(ServerConfig.GetString("ElasticSearch", "Fingerprint"));
                    if (ServerConfig.GetBoolean("ElasticCloud", "BasicAuth_Enable", false))
                    {
                        CPrint.WriteLine("[ElasticAdapter] Using URL Pool with Basic Auth (Username + Password)");
                        var basicAuth = new BasicAuthentication(
                            ServerConfig.GetString("ElasticCloud", "BasicAuth_Username", ""),
                            ServerConfig.GetString("ElasticCloud", "BasicAuth_Password"));
                        settings.Authentication(basicAuth);
                    }
                    else
                    {
                        CPrint.WriteLine("[ElasticAdapter] Using URL Pool with API Key");
                        settings.Authentication(new ApiKey(ServerConfig.GetString("ElasticSearch", "APIKey", "")));
                    }

                    Client = new ElasticsearchClient(
                        settings);
                    Initialize();
                }
            }
            else
            {
                CPrint.WriteLine("[ElasticAdapter] Disabled in ServerConfig (ElasticSearch.Enable is False)");
            }
        }


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
                    var reqData = new AuthorizedRequestEntry()
                    {
                        Username = account.Username,
                        Path = context.Request.Path,
                        UserAgent = context.Request.Headers.UserAgent,
                        Address = possibleAddress,
                        Method = context.Request.Method
                    };
                    if (GeolocationAdapter.Component != null)
                    {
                        var geoRes = GeolocationAdapter.Component.IPQuery(reqData.Address);
                        if (geoRes != null && geoRes.Status == "OK")
                            reqData.CountryCode = geoRes.CountryShort;
                    }
                    var res = Client?.Index(reqData, request => request.Index(indexName));
                    if (!res.IsSuccess())
                    {
                        CPrint.Error($"[ElasticAdapter.ASPMiddleware] Failed to index in \"{indexName}\"\n{res.ElasticsearchServerError.Error}");
                    }
                }
            }

            return next();
        }

        private static void AuditCreateHandle(AuditLogEntry entry)
        {
            if (!ServerConfig.GetBoolean("ElasticSearch", "Enable", false) || Client == null)
                return;

            IndexResponse? elasticResponse = null;
            string indexName = IndexPrefix + "auditLog-" + entry.ActionType.ToString();
            indexName = indexName.ToLower();
            switch (entry.ActionType)
            {
                case AuditType.AccountDelete:
                    var accountDeleteDeser = JsonSerializer.Deserialize<AccountDeleteEntryData>(entry.ActionData, OSLHelper.SerializerOptions);

                    elasticResponse = Client?.IndexAsync(new AccountDeleteEntry()
                    {
                        Username = entry.Username,
                        Timestamp = entry.Timestamp,
                        TargetUsername = accountDeleteDeser?.Username ?? "<none>"
                    }, request => request.Index(indexName)).Result;
                    break;
                case AuditType.AccountDisable:
                    var disabledeser = JsonSerializer.Deserialize<AccountDisableEntryData>(entry.ActionData, OSLHelper.SerializerOptions);

                    elasticResponse = Client?.IndexAsync(new AccountDisableEntry(disabledeser)
                    {
                        Username = entry.Username,
                        Timestamp = entry.Timestamp,
                        TargetUsername = disabledeser?.Username ?? "<none>",
                        State = disabledeser?.State ?? false,
                        Reason = disabledeser?.Reason ?? "<none>",
                    },
                    request => request.Index(indexName)).Result;
                    break;
                case AuditType.AnnouncementStateToggle:
                    var stateToggleDeser = JsonSerializer.Deserialize<AnnouncementStateToggleEntryData>(entry.ActionData, OSLHelper.SerializerOptions);

                    elasticResponse = Client?.IndexAsync(new AnnouncementStateToggleEntry(stateToggleDeser)
                    {
                        Username = entry.Username,
                        Timestamp = entry.Timestamp,
                        State = stateToggleDeser?.State ?? false
                    },
                    request => request.Index(indexName)).Result;
                    break;
                default:
                    CPrint.Warn($"[ElasticAdapter.AuditCreateHandle] Unsupported ActionType \"{entry.ActionType}\"");
                    return;
                    break;
            }

            if (elasticResponse != null)
            {
                if (elasticResponse.IsValidResponse)
                {
                    Console.WriteLine($"Index document with ID {elasticResponse.Id} succeeded.");
                }
            }
        }
    }
}
