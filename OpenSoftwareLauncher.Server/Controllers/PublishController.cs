using Microsoft.AspNetCore.Mvc;
using OSLCommon.AutoUpdater;
using System.Text.Json;
using Microsoft.AspNetCore.Http.Features;
using System.Net;
using OSLCommon;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using System.Collections.Generic;
using OSLCommon.Authorization;
using System.Linq;
using OSLCommon.Logging;

namespace OpenSoftwareLauncher.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PublishController : Controller
    {
        /// <summary>
        /// Body is required to be type of <see cref="PublishParameters"/>
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(401, Type = typeof(HttpException))]
        [ProducesResponseType(200, Type = typeof(Dictionary<string, bool>))]
        public ActionResult Index(string token)
        {
            if (!MainClass.ValidTokens.ContainsKey(token))
            {
                var account = MainClass.contentManager.AccountManager.GetAccount(token, true);
                bool allow = false;
                if (account != null)
                {
                    if (account.Enabled)
                    {
                        if (account.Permissions.Contains(AccountPermission.ADMINISTRATOR))
                        {
                            allow = true;
                        }
                    }
                    else
                    {
                        HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return new JsonResult(new ObjectResponse<HttpException>()
                        {
                            Success = false,
                            Data = new HttpException(StatusCodes.Status401Unauthorized, ServerStringResponse.AccountDisabled + "\n====Reason====\n" + account.DisableReasons.OrderBy(v => v.Timestamp).First()?.Message)
                        }, MainClass.serializerOptions);
                    }
                }
                if (!allow)
                {
                    Response.StatusCode = 401;
                    return Json(new HttpException(401, ServerStringResponse.InvalidCredential), MainClass.serializerOptions);
                }
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
            PublishParameters? decodedBody = null;
            try
            {
                StreamReader reader = new StreamReader(Request.Body);
                string decodedBodyText = reader.ReadToEnd();
                decodedBody = JsonSerializer.Deserialize<PublishParameters>(decodedBodyText, MainClass.serializerOptions);
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
            var parameters = decodedBody;
            if (parameters.releaseInfo.remoteLocation.Length < 1)
                parameters.releaseInfo.remoteLocation = $"{parameters.organization}/{parameters.product}-{parameters.branch}";

            for (int i = 0; i < parameters.files.Count; i++)
            {
                parameters.files[i].ETag = parameters.files[i].ETag.Replace("\"", @"");
            }
            var fileList = new List<PublishedReleaseFile>();
            var publishedRelease = new PublishedRelease()
            {
                CommitHash = parameters.releaseInfo.commitHash,
                Timestamp = parameters.releaseInfo.envtimestamp,
                Release = parameters.releaseInfo
            };
            foreach (var file in parameters.files)
            {
                fileList.Add(file.ToPublishedReleaseFile(publishedRelease.CommitHash));
            }
            publishedRelease.Files = fileList.ToArray();
            var result = new Dictionary<string, bool>()
            {
                { "alreadyPublished", true },
                { "releaseAlreadyExists", true },
                { "attemptSave", false }
            };
            bool saveRelease = MainClass.contentManager?.GetPublishedReleaseByHash(parameters.releaseInfo.commitHash) == null;
            bool saveReleaseInfo = !MainClass.contentManager?.GetReleaseInfoContent().ToList().Contains(parameters.releaseInfo) ?? false;
            if (saveRelease)
            {
                MainClass.contentManager?.SetPublishedRelease(publishedRelease);
                result["alreadyPublished"] = false;
            }
            if (saveReleaseInfo)
            {
                var cnt = MainClass.contentManager?.GetReleaseInfoContent().ToList() ?? new List<ReleaseInfo>();
                cnt.Add(parameters.releaseInfo);
                MainClass.contentManager.SetReleaseInfoContent(cnt.ToArray());
                result["releaseAlreadyExists"] = false;
            }

            MainClass.contentManager?.AuditLogManager.Create(new PublishReleaseEntryData(publishedRelease), null).Wait();

            return Json(result, MainClass.serializerOptions);
        }

        [HttpGet("all")]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<Dictionary<string, PublishedRelease>>))]
        public ActionResult All(string token)
        {
            var account = MainClass.contentManager.AccountManager.GetAccount(token, bumpLastUsed: true);
            if (!MainClass.ValidTokens.ContainsKey(token))
            {
                if (account == null)
                {
                    Response.StatusCode = 401;
                    return Json(new ObjectResponse<HttpException>()
                    {
                        Data = new HttpException(401, ServerStringResponse.InvalidCredential),
                        Success = false
                    }, MainClass.serializerOptions);
                }
                if (!account.Enabled)
                {
                    Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Json(new ObjectResponse<HttpException>()
                    {
                        Success = false,
                        Data = new HttpException(StatusCodes.Status401Unauthorized, ServerStringResponse.AccountDisabled)
                    }, MainClass.serializerOptions);
                }
                if (!account.HasPermission(AccountPermission.ADMINISTRATOR))
                {
                    Response.StatusCode = 401;
                    return Json(new ObjectResponse<HttpException>()
                    {
                        Data = new HttpException(401, ServerStringResponse.InvalidPermission),
                        Success = false
                    }, MainClass.serializerOptions);
                }
            }
            return Json(new ObjectResponse<Dictionary<string, PublishedRelease>>()
            {
                Data = MainClass.contentManager?.GetAllPublished() ?? new Dictionary<string, PublishedRelease>(),
                Success = true
            }, MainClass.serializerOptions);
        }

        [HttpGet("hash")]
        [ProducesResponseType(401, Type = typeof(HttpException))]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<PublishedRelease?>))]
        [OSLAuthRequired]
        [OSLAuthPermission(AccountPermission.ADMINISTRATOR)]
        public ActionResult ByCommitHashFromParameter(string hash, string? token)
        {
            PublishedRelease? commit = MainClass.contentManager?.GetPublishedReleaseByHash(hash);
            return Json(new ObjectResponse<PublishedRelease?>()
            {
                Data = commit,
                Success = true
            }, MainClass.serializerOptions);
        }

        [HttpGet("hash/{hash}")]
        [ProducesResponseType(401, Type = typeof(HttpException))]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<PublishedRelease?>))]
        [OSLAuthRequired]
        [OSLAuthPermission(AccountPermission.ADMINISTRATOR)]
        public ActionResult ByCommitHashFromPath(string? token, string hash) => ByCommitHashFromParameter(token ?? "", hash);
    }
}
