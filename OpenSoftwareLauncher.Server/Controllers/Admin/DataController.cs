using OSLCommon;
using OSLCommon.Authorization;
using OSLCommon.AutoUpdater;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace OpenSoftwareLauncher.Server.Controllers.Admin
{

    [Route("admin/[controller]")]
    [ApiController]
    [OSLAuthRequired]
    [OSLAuthPermission(AccountPermission.ADMINISTRATOR)]
    public class DataController : Controller
    {
        [HttpPost("restore")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<DataJSON>))]
        [ProducesResponseType(400, Type = typeof(ObjectResponse<HttpException>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        [ProducesResponseType(500, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult Restore(string token)
        {
            HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);
            string content = new StreamReader(HttpContext.Request.Body).ReadToEndAsync().Result.ReplaceLineEndings("");

            ObjectResponse<dynamic>? dynamicBody;
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
                MainClass.GetService<MongoAccountManager>()?.ReadJSON(JsonSerializer.Serialize(expectedResponse.Data.Account, MainClass.serializerOptions));
                MainClass.GetService<MongoSystemAnnouncement>()?.Read(JsonSerializer.Serialize(expectedResponse.Data.SystemAnnouncement, MainClass.serializerOptions));
                MainClass.GetService<MongoMiddle>()?.SetReleaseInfoContent(expectedResponse.Data.Content.ReleaseInfoContent.ToArray());
                MainClass.GetService<MongoMiddle>()?.ForceSetPublishedContent(expectedResponse.Data.Content.Published.Select(v => v.Value).ToArray());
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

            DataJSON content;

            try
            {
                content = new DataJSON()
                {
                    Account = MainClass.GetService<MongoAccountManager>()?.GetAllAccounts().ToList(),
                    SystemAnnouncement = MainClass.GetService<MongoSystemAnnouncement>()?.GetSummary(),
                    Content = new ContentJSON()
                    {
                        ReleaseInfoContent = MainClass.GetService<MongoMiddle>()?.GetReleaseInfoContent().ToList(),
                        Published = MainClass.GetService<MongoMiddle>()?.GetAllPublished()
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

            if (type == DataType.All)
            {
                return Json(new ObjectResponse<AllDataResult>()
                {
                    Success = true,
                    Data = new AllDataResult()
                    {
                        ReleaseInfoContent = MainClass.GetService<MongoMiddle>()?.GetReleaseInfoContent().ToList(),
                        Published = MainClass.GetService<MongoMiddle>()?.GetAllPublished()
                    }
                }, MainClass.serializerOptions);
            }
            else if (type == DataType.ReleaseInfoArray)
            {
                return Json(new ObjectResponse<ReleaseInfo[]>()
                {
                    Success = true,
                    Data = MainClass.GetService<MongoMiddle>()?.GetReleaseInfoContent() ?? Array.Empty<ReleaseInfo>()
                }, MainClass.serializerOptions);
            }
            else if (type == DataType.PublishDict)
            {
                return Json(new ObjectResponse<Dictionary<string, PublishedRelease>>()
                {
                    Success = true,
                    Data = MainClass.GetService<MongoMiddle>()?.GetAllPublished() ?? new Dictionary<string, PublishedRelease>()
                }, MainClass.serializerOptions); ;
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

            HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);
            string content = new StreamReader(HttpContext.Request.Body).ReadToEndAsync().Result.ReplaceLineEndings("");
            var deserializedDynamic = JsonSerializer.Deserialize<ObjectResponse<dynamic>>(WebUtility.UrlDecode(content), MainClass.serializerOptions);
            Type targetType = Type.GetType(deserializedDynamic.DataType);
            bool success = false;
            if (targetType == typeof(ReleaseInfo[]))
            {
                MainClass.GetService<MongoMiddle>()?.SetReleaseInfoContent(new List<ReleaseInfo>(deserializedDynamic.Data).ToArray());
                success = true;
            }
            else if (targetType == typeof(Dictionary<string, PublishedRelease>))
            {
                var des = JsonSerializer.Deserialize<ObjectResponse<Dictionary<string, PublishedRelease>>>(content, MainClass.serializerOptions);
                MainClass.GetService<MongoMiddle>()?.ForceSetPublishedContent(des?.Data.Select(v => v.Value).ToArray() ?? Array.Empty<PublishedRelease>());
                success = true;
            }
            else if (targetType == typeof(AllDataResult))
            {
                var des = JsonSerializer.Deserialize<ObjectResponse<AllDataResult>>(content, MainClass.serializerOptions);

                var c = des?.Data;
                MainClass.GetService<MongoMiddle>()?.SetReleaseInfoContent(new List<ReleaseInfo>(c?.ReleaseInfoContent ?? new List<ReleaseInfo>()).ToArray());
                MainClass.GetService<MongoMiddle>()?.ForceSetPublishedContent(c?.Published.Select(v => v.Value).ToArray() ?? Array.Empty<PublishedRelease>());
                success = true;
            }

            if (success)
            {
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
