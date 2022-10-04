using OSLCommon;
using OSLCommon.Authorization;
using OSLCommon.AutoUpdater;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IO;

namespace OpenSoftwareLauncher.Server.Controllers.Admin
{

    [Route("admin/[controller]")]
    [ApiController]
    public class DataController : Controller
    {
        [HttpPost("restore")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<DataJSON>))]
        [ProducesResponseType(400, Type = typeof(ObjectResponse<HttpException>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        [ProducesResponseType(500, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult Restore(string token)
        {
            if (!MainClass.contentManager.AccountManager.AccountHasPermission(token, AccountPermission.ADMINISTRATOR))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(401, ServerStringResponse.InvalidCredential)
                }, MainClass.serializerOptions);
            }
            var tokenAccount = MainClass.contentManager.AccountManager.GetAccount(token, bumpLastUsed: true);
            if (tokenAccount == null)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(401, ServerStringResponse.InvalidCredential)
                }, MainClass.serializerOptions);
            }
            if (!tokenAccount.Enabled)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(401, ServerStringResponse.AccountDisabled)
                }, MainClass.serializerOptions);
            }

            HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);
            string content = new StreamReader(HttpContext.Request.Body).ReadToEndAsync().Result.ReplaceLineEndings("");

            ObjectResponse<dynamic> dynamicBody;
            try
            {
                dynamicBody = JsonSerializer.Deserialize<ObjectResponse<dynamic>>(content, MainClass.serializerOptions);
            }
            catch (Exception except)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(400, ServerStringResponse.InvalidBody, except)
                }, MainClass.serializerOptions);
            }
            if (dynamicBody == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(400, ServerStringResponse.InvalidBody)
                }, MainClass.serializerOptions);
            }
            if (dynamicBody.Success == false)
            {

                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(400, ServerStringResponse.ExpectedValueOnProperty("Success", true, false))
                }, MainClass.serializerOptions);
            }

            ObjectResponse<DataJSON>? expectedResponse = JsonSerializer.Deserialize<ObjectResponse<DataJSON>>(content, MainClass.serializerOptions);
            if (expectedResponse == null)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(400, ServerStringResponse.InvalidBody)
                }, MainClass.serializerOptions);
            }

            try
            {
                MainClass.contentManager.AccountManager.Read(JsonSerializer.Serialize(expectedResponse.Data.Account, MainClass.serializerOptions));
                MainClass.contentManager.SystemAnnouncement.Read(JsonSerializer.Serialize(expectedResponse.Data.SystemAnnouncement, MainClass.serializerOptions));
                MainClass.contentManager.ReleaseInfoContent = expectedResponse.Data.Content.ReleaseInfoContent;
                MainClass.contentManager.Releases = expectedResponse.Data.Content.Releases;
                MainClass.contentManager.Published = expectedResponse.Data.Content.Published;
                MainClass.contentManager.DatabaseSerialize();
            }
            catch (Exception except)
            {
                Response.StatusCode = 500;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(500, ServerStringResponse.SerializationFailure, except)
                }, MainClass.serializerOptions);
            }

            return Json(new ObjectResponse<DataJSON>()
            {
                Success = true,
                Data = expectedResponse.Data
            }, MainClass.serializerOptions);
        }

        [HttpGet("fetch")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<DataJSON>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        [ProducesResponseType(500, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult Fetch(string token)
        {
            if (!MainClass.contentManager.AccountManager.AccountHasPermission(token, AccountPermission.ADMINISTRATOR))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(401, ServerStringResponse.InvalidCredential)
                }, MainClass.serializerOptions);
            }
            var tokenAccount = MainClass.contentManager.AccountManager.GetAccount(token, bumpLastUsed: true);
            if (tokenAccount == null)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(401, ServerStringResponse.InvalidCredential)
                }, MainClass.serializerOptions);
            }
            if (!tokenAccount.Enabled)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(401, ServerStringResponse.AccountDisabled)
                }, MainClass.serializerOptions);
            }

            DataJSON content;

            try
            {
                content = new DataJSON()
                {
                    Account = MainClass.contentManager.AccountManager.AccountList,
                    SystemAnnouncement = MainClass.contentManager.SystemAnnouncement.GetSummary(),
                    Content = new ContentJSON()
                    {
                        ReleaseInfoContent = MainClass.contentManager.ReleaseInfoContent,
                        Releases = MainClass.contentManager.Releases,
                        Published = MainClass.contentManager.Published
                    }
                };
            }
            catch (Exception except)
            {
                Response.StatusCode = 500;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(500, $"Failed to generate content", except)
                }, MainClass.serializerOptions);
            }


            return Json(new ObjectResponse<DataJSON>()
            {
                Success = true,
                Data = content
            }, MainClass.serializerOptions);
        }

        [HttpGet("dump")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<DataJSON>))]
        [ProducesResponseType(400, Type = typeof(ObjectResponse<HttpException>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult Dump(string token, DataType type)
        {
            if (!MainClass.contentManager.AccountManager.AccountHasPermission(token, AccountPermission.ADMINISTRATOR))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(401, ServerStringResponse.InvalidCredential)
                }, MainClass.serializerOptions);
            }
            var tokenAccount = MainClass.contentManager.AccountManager.GetAccount(token, bumpLastUsed: true);
            if (tokenAccount == null)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(401, ServerStringResponse.InvalidCredential)
                }, MainClass.serializerOptions);
            }
            if (!tokenAccount.Enabled)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(401, ServerStringResponse.AccountDisabled)
                }, MainClass.serializerOptions);
            }

            if (type == DataType.All)
            {
                return Json(new ObjectResponse<AllDataResult>()
                {
                    Success = true,
                    Data = new AllDataResult()
                    {
                        ReleaseInfoContent = MainClass.contentManager.ReleaseInfoContent,
                        Releases = MainClass.contentManager.Releases,
                        Published = MainClass.contentManager.Published
                    }
                }, MainClass.serializerOptions);
            }
            else if (type == DataType.ReleaseInfoArray)
            {
                return Json(new ObjectResponse<ReleaseInfo[]>()
                {
                    Success = true,
                    Data = MainClass.contentManager.ReleaseInfoContent.ToArray()
                }, MainClass.serializerOptions);
            }
            else if (type == DataType.ReleaseDict)
            {
                return Json(new ObjectResponse<Dictionary<string, ProductRelease>>()
                {
                    Success = true,
                    Data = MainClass.contentManager.Releases
                }, MainClass.serializerOptions);
            }
            else if (type == DataType.PublishDict)
            {
                return Json(new ObjectResponse<Dictionary<string, PublishedRelease>>()
                {
                    Success = true,
                    Data = MainClass.contentManager.Published
                }, MainClass.serializerOptions);
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(400, ServerStringResponse.InvalidParameter("type"))
                }, MainClass.serializerOptions);
            }
        }

        [HttpPost("setdata")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<DataJSON>))]
        [ProducesResponseType(400, Type = typeof(ObjectResponse<HttpException>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult SetData(string token)
        {
            if (!MainClass.contentManager.AccountManager.AccountHasPermission(token, AccountPermission.ADMINISTRATOR))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(401, ServerStringResponse.InvalidCredential)
                }, MainClass.serializerOptions);
            }
            var tokenAccount = MainClass.contentManager.AccountManager.GetAccount(token, bumpLastUsed: true);
            if (tokenAccount == null)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(401, ServerStringResponse.InvalidCredential)
                }, MainClass.serializerOptions);
            }
            if (!tokenAccount.Enabled)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(401, ServerStringResponse.AccountDisabled)
                }, MainClass.serializerOptions);
            }
            HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);
            string content = new StreamReader(HttpContext.Request.Body).ReadToEndAsync().Result.ReplaceLineEndings("");
            var deserializedDynamic = JsonSerializer.Deserialize<ObjectResponse<dynamic>>(WebUtility.UrlDecode(content), MainClass.serializerOptions);
            Type targetType = Type.GetType(deserializedDynamic.DataType);
            bool success = false;
            if (targetType == typeof(ReleaseInfo[]))
            {
                MainClass.contentManager.ReleaseInfoContent = new List<ReleaseInfo>(deserializedDynamic.Data);
                success = true;
            }
            else if (targetType == MainClass.contentManager.Releases.GetType())
            {
                var des = JsonSerializer.Deserialize<ObjectResponse<Dictionary<string, ProductRelease>>>(content, MainClass.serializerOptions);
                MainClass.contentManager.Releases = des.Data;
                success = true;
            }
            else if (targetType == MainClass.contentManager.Published.GetType())
            {
                var des = JsonSerializer.Deserialize<ObjectResponse<Dictionary<string, PublishedRelease>>>(content, MainClass.serializerOptions);
                MainClass.contentManager.Published = des.Data;
                success = true;
            }
            else if (targetType == typeof(AllDataResult))
            {
                var des = JsonSerializer.Deserialize<ObjectResponse<AllDataResult>>(content, MainClass.serializerOptions);

                var c = des.Data;
                MainClass.contentManager.ReleaseInfoContent = new List<ReleaseInfo>(c.ReleaseInfoContent);
                MainClass.contentManager.Releases = c.Releases;
                MainClass.contentManager.Published = c.Published;
                success = true;
            }

            if (success)
            {
                MainClass.contentManager.DatabaseSerialize();
                return Json(new ObjectResponse<object>()
                {
                    Success = true,
                    Data = deserializedDynamic.Data,
                    DataType = deserializedDynamic.DataType
                }, MainClass.serializerOptions);
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(400, ServerStringResponse.InvalidParameter("type"))
                }, MainClass.serializerOptions);
            }
        }
    }
}
