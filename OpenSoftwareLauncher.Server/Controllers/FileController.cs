using OSLCommon;
using OSLCommon.AutoUpdater;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using OSLCommon.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace OpenSoftwareLauncher.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : Controller
    {
        [HttpPost("{hash}")]
        [ProducesResponseType(401, Type = typeof(HttpException))]
        [ProducesResponseType(404, Type = typeof(HttpException))]
        [ProducesResponseType(200, Type = typeof(PublishedReleaseFile[]))]
        [ProducesResponseType(500, Type = typeof(HttpException))]
        public ActionResult AddFileToHash(string hash, string token)
        {
            var mongoMiddle = MainClass.GetService<MongoMiddle>();
            if (token == null || token.Length < 1 || !MainClass.ValidTokens.ContainsKey(token) || (!MainClass.GetService<MongoAccountManager>()?.AccountHasPermission(token, AccountPermission.RELEASE_MANAGE) ?? false))
            {
                Response.StatusCode = 401;
                return Json(new HttpException(401, ServerStringResponse.InvalidCredential), MainClass.serializerOptions);
            }
            else if (mongoMiddle?.GetPublishedReleaseByHash(hash) == null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return Json(new HttpException(404, @"Commit not published"), MainClass.serializerOptions);
            }
            if (!Request.HasJsonContentType())
            {
                Response.StatusCode = (int)HttpStatusCode.UnsupportedMediaType;
                return Json(new HttpException(401, ServerStringResponse.UnsupportedMediaType), MainClass.serializerOptions);
            }
            var syncIOFeature = HttpContext.Features.Get<IHttpBodyControlFeature>();
            if (syncIOFeature != null)
            {
                syncIOFeature.AllowSynchronousIO = true;
            }

            List<ManagedUploadSendData>? decodedBody = null;
            try
            {
                StreamReader reader = new StreamReader(Request.Body);
                string decodedBodyText = reader.ReadToEnd();
                decodedBody = JsonSerializer.Deserialize<List<ManagedUploadSendData>>(decodedBodyText, MainClass.serializerOptions);
            }
            catch (Exception e)
            {
                Response.StatusCode = 401;
                return Json(new HttpException(401, ServerStringResponse.InvalidBody, e), MainClass.serializerOptions);
            }
            if (decodedBody == null)
            {
                Response.StatusCode = 401;
                return Json(new HttpException(401, ServerStringResponse.InvalidBody), MainClass.serializerOptions);
            }

            var fileList = new List<PublishedReleaseFile>();
            foreach (var i in decodedBody)
                fileList.Add(i.ToPublishedReleaseFile(hash));
            if (mongoMiddle == null)
            {
                Response.StatusCode = 500;
                return Json(new HttpException(500, "Content Manager has not been initalized"), MainClass.serializerOptions);
            }
            mongoMiddle?.AddPublishedFilesByHash(hash, fileList.ToArray());
            var commit = mongoMiddle?.GetPublishedReleaseByHash(hash);
            return Json(commit?.Files ?? Array.Empty<object>(), MainClass.serializerOptions);
        }

        [HttpGet("{hash}")]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        [ProducesResponseType(200, Type = typeof(List<PublishedReleaseFile>))]
        [OSLAuthRequired]
        public ActionResult FetchFilesFromHash(string hash, string token)
        {
            MongoMiddle? mongoMiddle = MainClass.GetService<MongoMiddle>();
            var returnContent = new List<PublishedReleaseFile>();

            var account = MainClass.GetService<MongoAccountManager>()?.GetAccount(token, true);

            if (mongoMiddle != null)
            {
                var commit = mongoMiddle.GetPublishedReleaseByHash(hash);
                if (commit != null)
                {
                    var allow = false;
                    if (account != null)
                    {
                        /*if (commit.Release.releaseType != ReleaseType.Other)
                        {
                            allow = MainClass.CanUserGroupsAccessStream(commit.Release.groupBlacklist.ToArray(), commit.Release.groupWhitelist.ToArray(), account);
                        }*/
                        if (MainClass.Config.Security.AllowAdminOverride)
                        {
                            if (account.HasPermission(OSLCommon.Authorization.AccountPermission.ADMINISTRATOR))
                                allow = true;
                        }

                        if (commit.Release.releaseType == ReleaseType.Other)
                        {
                            if (MainClass.Config.Security.AllowPermissionReadReleaseBypass)
                                if (account.HasPermission(OSLCommon.Authorization.AccountPermission.READ_RELEASE_BYPASS))
                                    allow = true;
                        }

                        if (account.HasLicense(commit.Release.remoteLocation))
                        {
                            allow = true;
                        }
                    }
                    if (AccountManager.DefaultLicenses.Contains(commit.Release.remoteLocation))
                        allow = true;
                    if (allow)
                        returnContent = new List<PublishedReleaseFile>(commit.Files);

                }
            }
            return Json(returnContent, MainClass.serializerOptions);
        }

        [HttpGet("")]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        [ProducesResponseType(200, Type = typeof(List<PublishedReleaseFile>))]
        [OSLAuthRequired]
        public ActionResult FetchFilesFromHashByParameter(string hash, string? token = "") => FetchFilesFromHash(hash, token ?? "");
    }
}
