using Microsoft.AspNetCore.Mvc;
using OSLCommon.AutoUpdater;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using OSLCommon;
using OSLCommon.Authorization;
using OpenSoftwareLauncher.Server.OpenSoftwareLauncher.Server;
using System.Net;
using System;
using Microsoft.AspNetCore.Http;

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
                        Data = MainClass.contentManager?.Releases ?? new Dictionary<string, ProductRelease>()
                    }, MainClass.serializerOptions);
                }
                if (ServerConfig.GetBoolean("Security", "AllowAdminOverride", true) && account.HasPermission(AccountPermission.ADMINISTRATOR))
                {
                    return Json(new ObjectResponse<Dictionary<string, ProductRelease>>()
                    {
                        Success = true,
                        Data = MainClass.contentManager?.Releases ?? new Dictionary<string, ProductRelease>()
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
            if (allowFetch && (MainClass.contentManager?.Releases.ContainsKey(app) ?? false))
            {
                var toMap = new Dictionary<string, List<ReleaseInfo>>();
                foreach (var release in MainClass.contentManager.ReleaseInfoContent)
                {
                    if (app != release.appID) continue;
                    if (!toMap.ContainsKey(release.remoteLocation))
                        toMap.Add(release.remoteLocation, new List<ReleaseInfo>());
                    toMap[release.remoteLocation].Add(release);
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
            var account = MainClass.contentManager.AccountManager.GetAccount(token);
            if (account == null && token.Length > 0)
            {
                returnContent.Clear();
            }
            else
            {
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
                                isOtherBranch = false;
                                break;
                            case "beta":
                                isOtherBranch = false;
                                break;
                            case "stable":
                                isOtherBranch = false;
                                break;
                        }
                        bool allowStream = false;
                        if (ServerConfig.GetBoolean("Security", "AllowGroupRestriction", false))
                        {
                            if (account == null)
                                allowStream = false;
                            else
                                allowStream = MainClass.CanUserGroupsAccessStream(stream.GroupBlacklist.ToArray(), stream.GroupWhitelist.ToArray(), account);
                        }
                        else
                        {
                            allowStream = true;
                        }

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

                        
                        if (allowStream)
                            filteredStreams.Add(stream);
                    }
                    newProduct.Streams = filteredStreams.ToArray();
                    filteredReleases.Add(newProduct);
                }
                returnContent = filteredReleases;
            }

            return returnContent.Where(v => v.Streams.Length > 0).ToList();
        }

        private List<ProductRelease> fetchAllReleases(string token)
        {
            var appIDlist = new List<string>();
            foreach (var k in MainClass.contentManager.ReleaseInfoContent)
                appIDlist.Add(k.appID);
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
    }
}
