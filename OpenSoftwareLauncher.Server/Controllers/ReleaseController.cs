using Microsoft.AspNetCore.Mvc;
using OSLCommon.AutoUpdater;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using OSLCommon;
using OSLCommon.Authorization;
using System;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using System.Runtime.Serialization;

namespace OpenSoftwareLauncher.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReleaseController : Controller
    {
        [HttpGet]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<Dictionary<string, ProductRelease>>))]
        public ActionResult Index(string token)
        {
            var account = MainClass.contentManager.AccountManager.GetAccount(token, bumpLastUsed: true);
            if (account != null)
            {
                if (!account.Enabled)
                {
                    Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Json(new ObjectResponse<HttpException>()
                    {
                        Success = false,
                        Data = new HttpException(StatusCodes.Status401Unauthorized, ServerStringResponse.AccountDisabled)
                    }, MainClass.serializerOptions);
                }
                if (ServerConfig.GetBoolean("Security", "AllowPermission_ReadReleaseBypass", true) && account.HasPermission(AccountPermission.READ_RELEASE_BYPASS))
                {
                    return Json(new ObjectResponse<Dictionary<string, ProductRelease>>()
                    {
                        Success = true,
                        Data = ReleaseHelper.TransformReleaseList(MainClass.contentManager?.GetReleaseInfoContent())
                    }, MainClass.serializerOptions);
                }
                if (ServerConfig.GetBoolean("Security", "AllowAdminOverride", true) && account.HasPermission(AccountPermission.ADMINISTRATOR))
                {
                    return Json(new ObjectResponse<Dictionary<string, ProductRelease>>()
                    {
                        Success = true,
                        Data = ReleaseHelper.TransformReleaseList(MainClass.contentManager.GetReleaseInfoContent())
                    }, MainClass.serializerOptions);
                }
            }
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Json(new ObjectResponse<HttpException>()
            {
                Success = false,
                Data = new HttpException(401, ServerStringResponse.InvalidCredential)
            }, MainClass.serializerOptions);
        }

        private List<ProductRelease> fetchReleasesByAppID(string app, string token)
        {
            var returnContent = new List<ProductRelease>();
            var allowFetch = true;
            bool showExtraBuilds = MainClass.contentManager.AccountManager.AccountHasPermission(token, AccountPermission.READ_RELEASE_BYPASS)
                && ServerConfig.GetBoolean("Security", "AllowPermission_ReadReleaseBypass", true);
            bool contains = MainClass.contentManager.MongoClient.GetDatabase(ContentManager.DatabaseName)
                    .GetCollection<ReleaseInfo>(ContentManager.ReleaseInfo_Collection)
                    .Find(Builders<ReleaseInfo>
                        .Filter
                        .Eq("appID", app))
                    .ToList()
                    .Count > 0;
            if (allowFetch && contains)
            {
                var toMap = new Dictionary<string, List<ReleaseInfo>>();

                var cnt = MainClass.contentManager.MongoClient.GetDatabase(ContentManager.DatabaseName)
                    .GetCollection<ReleaseInfo>(ContentManager.ReleaseInfo_Collection)
                    .Find(Builders<ReleaseInfo>
                        .Filter
                        .Eq("appID", app))
                    .ToList();
                foreach (var item in cnt)
                {
                    if (!toMap.ContainsKey(item.remoteLocation))
                        toMap.Add(item.remoteLocation, new List<ReleaseInfo>());
                    toMap[item.remoteLocation].Add(item);
                }

                var sortedDictionary = new Dictionary<string, List<ReleaseInfo>>();
                foreach (var pair in toMap)
                {
                    sortedDictionary.Add(pair.Key, pair.Value.OrderBy(o => o.envtimestamp).Reverse().ToList());
                }

                var latestOfAll = new List<ReleaseInfo>();
                foreach (var pair in sortedDictionary)
                {
                    latestOfAll.Add(pair.Value.First());
                }

                var latestOfAllArray = latestOfAll.ToArray();
                foreach (var pair in ReleaseHelper.TransformReleaseList(latestOfAllArray))
                {
                    var rel = new ProductRelease()
                    {
                        ProductID = pair.Value.ProductID,
                        ProductName = pair.Value.ProductName,
                        UID = pair.Value.UID
                    };
                    var tmpStreams = pair.Value.Streams;
                    var streams = new List<ProductReleaseStream>();
                    foreach (var s in tmpStreams)
                    {
                        streams.Add(s);
                    }
                    rel.Streams = streams.ToArray();
                    returnContent.Add(rel);
                }
            }

            var filteredReleases = new List<ProductRelease>();
            var account = MainClass.contentManager?.AccountManager.GetAccount(token);
            foreach (var product in returnContent)
            {
                var newProduct = new ProductRelease()
                {
                    ProductName = product.ProductName,
                    ProductID = product.ProductID,
                    UID = product.UID
                };
                var filteredStreams = new List<ProductReleaseStream>();
                foreach (var stream in product.Streams)
                {
                    bool isOtherBranch = true;
                    switch (stream.BranchName.ToLower())
                    {
                        case "nightly":
                        case "beta":
                        case "stable":
                            isOtherBranch = false;
                            break;
                    }
                    bool allowStream = false;
                    /*if (ServerConfig.GetBoolean("Security", "AllowGroupRestriction", false))
                    {
                        if (account == null)
                            allowStream = false;
                        else
                            allowStream = MainClass.CanUserGroupsAccessStream(stream.GroupBlacklist.ToArray(), stream.GroupWhitelist.ToArray(), account);
                    }
                    else
                    {
                        allowStream = true;
                    }*/

                    if (account != null)
                    {
                        if (ServerConfig.GetBoolean("Security", "AllowAdminOverride", true))
                        {
                            if (account != null && account.HasPermission(AccountPermission.ADMINISTRATOR))
                                allowStream = true;
                        }

                        if (isOtherBranch)
                        {
                            if (ServerConfig.GetBoolean("Security", "AllowPermission_ReadReleaseBypass", true))
                                if (account != null && account.HasPermission(AccountPermission.READ_RELEASE_BYPASS))
                                    allowStream = true;
                                else
                                    allowStream = false;
                        }

                        if (account != null && account.HasLicense(stream.RemoteSignature))
                        {
                            allowStream = true;
                        }
                    }

                    if (AccountManager.DefaultLicenses.Contains(stream.RemoteSignature))
                        allowStream = true;

                    if (allowStream)
                        filteredStreams.Add(stream);
                }
                newProduct.Streams = filteredStreams.ToArray();
                filteredReleases.Add(newProduct);
            }
            returnContent = filteredReleases;

            return returnContent.Where(v => v.Streams.Length > 0).ToList();
        }

        private List<ProductRelease> fetchAllReleases(string token)
        {
            var appIDlist = MainClass.contentManager.MongoClient.GetDatabase(ContentManager.DatabaseName)
                    .GetCollection<ReleaseInfo>(ContentManager.ReleaseInfo_Collection)
                    .Find(Builders<ReleaseInfo>.Filter.Empty)
                    .ToList()
                    .Select(v => v.appID)
                    .Distinct()
                    .ToList();
            appIDlist = appIDlist.Where(v => v.Length > 0).Distinct().ToList();

            var resultList = new List<ProductRelease>();
            foreach (var v in appIDlist)
                resultList = resultList.Concat(fetchReleasesByAppID(v, token ?? "")).ToList();
            return resultList;
        }

        [HttpGet("latest/{app}")]
        [ProducesResponseType(200, Type = typeof(List<ProductRelease>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult LatestFromPath(string app, string? token = "")
        {
            var account = MainClass.contentManager.AccountManager.GetAccount(token, bumpLastUsed: true);
            if (account != null && !account.Enabled)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(StatusCodes.Status401Unauthorized, ServerStringResponse.AccountDisabled)
                }, MainClass.serializerOptions);
            }
            return Json(fetchReleasesByAppID(app, token ?? ""), MainClass.serializerOptions);
        }

        [HttpGet("latest")]
        [ProducesResponseType(200, Type = typeof(List<ProductRelease>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult LatestFromParameter(string id="", string? token = "")
        {
            var account = MainClass.contentManager.AccountManager.GetAccount(token, bumpLastUsed: true);
            if (account != null && !account.Enabled)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(StatusCodes.Status401Unauthorized, ServerStringResponse.AccountDisabled)
                }, MainClass.serializerOptions);
            }
            // Get all latest available
            if (id.Length < 1)
            {
                return Json(fetchAllReleases(token ?? ""), MainClass.serializerOptions);
            }
            else
            {
                return LatestFromPath(id, token ?? "");
            }
        }

        [HttpGet("hasUpdate")]
        [Produces(typeof(ReleaseStreamUpdateDetails))]
        public ActionResult ReleaseHasUpdate(string app, string branch, string currentHash, string? token="")
        {
            ProductReleaseStream? targetReleaseStream = null;

            var result = fetchReleasesByAppID(app, token ?? "");
            foreach (var product in result)
            {
                if (product.ProductID != app) continue;
                foreach (var stream in product.Streams)
                {
                    if (stream.BranchName != branch) continue;
                    targetReleaseStream = stream;
                }
            }

            var details = new ReleaseStreamUpdateDetails()
            {
                Stream = targetReleaseStream,

                TargetApp = app,
                TargetBranch = branch,
                CurrentHash = currentHash
            };

            if (targetReleaseStream != null)
            {
                details.HasUpdate = targetReleaseStream.CommitHash != currentHash;
                details.UpdateReleaseTimestamp = targetReleaseStream.UpdatedTimestamp;
            }

            return Json(details, MainClass.serializerOptions);
        }
    
        
        [HttpGet("history/at")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<List<ProductReleaseStream>>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult FromCommitHash(string token, string hash, string? signature = "")
        {
            var validationResponse = MainClass.ValidatePermissions(token, AccountPermission.READ_RELEASE_HISTORY);
            if (validationResponse != null)
            {
                Response.StatusCode = validationResponse.Data.Code;
                return Json(validationResponse, MainClass.serializerOptions);
            }

            OSLCommon.Authorization.Account account = MainClass.contentManager.AccountManager.GetAccount(token);

            var collection = MainClass.contentManager.GetReleaseCollection();
            var filter = Builders<ReleaseInfo>
                .Filter
                .Eq("commitHash", hash);
            var foundList = collection.Find(filter).ToList();
            var result = foundList
                .Where((v) => 
                {
                    return account.HasLicense(v.remoteLocation);
                });

            if (signature != null && signature.Length > 0)
                result = result.Where(v => v.remoteLocation == signature);

            var resultArray = result.ToArray();
            var transformed = ReleaseHelper.TransformReleaseList(result.ToArray());

            var streamList = new List<ProductReleaseStream>();
            foreach (var pair in transformed)
                foreach (var item in pair.Value.Streams)
                    streamList.Add(item);

            return Json(new ObjectResponse<List<ProductReleaseStream>>()
            {
                Success = true,
                Data = streamList
            }, MainClass.serializerOptions);
        }

        [HttpGet("history/sig")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<List<ProductReleaseStream>>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult FromSignature(string token, string signature)
        {
            var validationResponse = MainClass.ValidatePermissions(token, AccountPermission.READ_RELEASE_HISTORY);
            if (validationResponse != null)
            {
                Response.StatusCode = validationResponse.Data.Code;
                return Json(validationResponse, MainClass.serializerOptions);
            }

            OSLCommon.Authorization.Account account = MainClass.contentManager.AccountManager.GetAccount(token);

            var collection = MainClass.contentManager.GetReleaseCollection();
            var filter = Builders<ReleaseInfo>
                .Filter
                .Eq("remoteLocation", signature);
            var foundList = collection.Find(filter).ToList();
            var result = foundList
                .Where((v) =>
                {
                    return account.HasLicense(v.remoteLocation);
                });

            if (signature != null && signature.Length > 0)
                result = result.Where(v => v.remoteLocation == signature);

            var resultArray = result.ToArray();
            var transformed = ReleaseHelper.TransformReleaseList(result.ToArray());

            var streamList = new List<ProductReleaseStream>();
            foreach (var pair in transformed)
                foreach (var item in pair.Value.Streams)
                    streamList.Add(item);

            return Json(new ObjectResponse<List<ProductReleaseStream>>()
            {
                Success = true,
                Data = streamList.OrderByDescending(v => v.UpdatedTimestamp).ToList()
            }, MainClass.serializerOptions);
        }
    }
}
