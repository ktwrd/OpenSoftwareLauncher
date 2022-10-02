using OpenSoftwareLauncher.Server.OpenSoftwareLauncher.Server;
using OSLCommon;
using OSLCommon.AutoUpdater;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;

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
            if (token == null || token.Length < 1 || !MainClass.ValidTokens.ContainsKey(token) || !MainClass.contentManager.AccountManager.AccountHasPermission(token, OSLCommon.Authorization.AccountPermission.RELEASE_MANAGE))
            {
                Response.StatusCode = 401;
                return Json(new HttpException(401, ServerStringResponse.InvalidCredential), MainClass.serializerOptions);
            }
            else if (!MainClass.contentManager?.Published.ContainsKey(hash) ?? false)
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
            if (MainClass.contentManager == null)
            {
                Response.StatusCode = 500;
                return Json(new HttpException(500, "Content Manager has not been initalized"), MainClass.serializerOptions);
            }
            MainClass.contentManager.Published[hash].Files =
            new List<PublishedReleaseFile>(MainClass.contentManager?.Published[hash].Files ?? new PublishedReleaseFile[] { })
                .Concat(fileList)
                .ToArray();
            return Json(MainClass.contentManager?.Published[hash].Files ?? Array.Empty<object>(), MainClass.serializerOptions);
        }

        [HttpGet("{hash}")]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        [ProducesResponseType(200, Type = typeof(List<PublishedReleaseFile>))]
        public ActionResult FetchFilesFromHash(string hash, string token)
        {
            var returnContent = new List<PublishedReleaseFile>();
            var contentManager = MainClass.contentManager;
            var account = MainClass.contentManager.AccountManager.GetAccount(token);
            if (!account.Enabled)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(StatusCodes.Status401Unauthorized, ServerStringResponse.AccountDisabled)
                }, MainClass.serializerOptions);
            }
            if (contentManager != null && account != null)
            {
                if (contentManager.Published.ContainsKey(hash))
                {
                    var allow = false;
                    var commit = contentManager.Published[hash];
                    if (commit.Release.releaseType != ReleaseType.Other)
                    {
                        allow = MainClass.CanUserGroupsAccessStream(commit.Release.groupBlacklist.ToArray(), commit.Release.groupWhitelist.ToArray(), account);
                    }
                    if (ServerConfig.GetBoolean("Security", "AllowAdminOverride", true))
                    {
                        if (account.HasPermission(OSLCommon.Authorization.AccountPermission.ADMINISTRATOR))
                            allow = true;
                    }

                    if (commit.Release.releaseType == ReleaseType.Other)
                    {
                        if (ServerConfig.GetBoolean("Security", "AllowPermission_ReadReleaseBypass", true))
                            if (account.HasPermission(OSLCommon.Authorization.AccountPermission.READ_RELEASE_BYPASS))
                                allow = true;
                    }

                    if (allow)
                        returnContent = new List<PublishedReleaseFile>(commit.Files);
                }
            }
            return Json(returnContent, MainClass.serializerOptions);
        }

        [HttpGet("")]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        [ProducesResponseType(200, Type = typeof(List<PublishedReleaseFile>))]
        public ActionResult FetchFilesFromHashByParameter(string hash, string? token = "") => FetchFilesFromHash(hash, token ?? "");
    }
}
