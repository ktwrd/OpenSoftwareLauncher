using OSLCommon;
using OSLCommon.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;

namespace OpenSoftwareLauncher.Server.Controllers.Admin
{
    [Route("admin/[controller]")]
    [ApiController]
    [OSLAuthRequired]
    [OSLAuthPermission(AccountPermission.ADMINISTRATOR)]
    public class ConfigController : Controller
    {

        [HttpGet("get")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<Dictionary<string, Dictionary<string, object>>>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult GetConfig(string token)
        {
            return Json(new ObjectResponse<Dictionary<string, Dictionary<string, object>>>()
            {
                Data = ServerConfig.Get(),
                Success = true
            }, MainClass.serializerOptions);
        }


        [HttpGet("setvalue")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<Dictionary<string, Dictionary<string, object>>>))]
        [ProducesResponseType(400, Type = typeof(ObjectResponse<HttpException>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult SetValue_Get(string token, string group, string key, object value)
        {
            ServerConfig.Set(group, key, value);

            return Json(new ObjectResponse<Dictionary<string, Dictionary<string, object>>>()
            {
                Data = ServerConfig.Get(),
                Success = true
            }, MainClass.serializerOptions);
        }

        [HttpPost("setvalue")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<Dictionary<string, Dictionary<string, object>>>))]
        [ProducesResponseType(400, Type = typeof(ObjectResponse<HttpException>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult SetValue(string token, string group, string key)
        {
            var syncIOFeature = HttpContext.Features.Get<IHttpBodyControlFeature>();
            if (syncIOFeature != null)
            {
                syncIOFeature.AllowSynchronousIO = true;
            }
            object? decodedBody;
            try
            {
                StreamReader reader = new StreamReader(Request.Body);
                string decodedBodyText = reader.ReadToEnd();
                decodedBody = JsonSerializer.Deserialize<object>(decodedBodyText, MainClass.serializerOptions);
            }
            catch (Exception e)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(StatusCodes.Status400BadRequest, ServerStringResponse.InvalidBody, e)
                }, MainClass.serializerOptions);
            }

            ServerConfig.Set(group, key, decodedBody ?? "");

            return Json(new ObjectResponse<Dictionary<string, Dictionary<string, object>>>()
            {
                Data = ServerConfig.Get(),
                Success = true
            }, MainClass.serializerOptions);
        }

        [HttpPost("set")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<Dictionary<string, Dictionary<string, object>>>))]
        [ProducesResponseType(400, Type = typeof(ObjectResponse<HttpException>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult SetConfig(string token)
        {
            var syncIOFeature = HttpContext.Features.Get<IHttpBodyControlFeature>();
            if (syncIOFeature != null)
            {
                syncIOFeature.AllowSynchronousIO = true;
            }
            Dictionary<string, Dictionary<string, object>>? decodedBody;
            try
            {
                StreamReader reader = new StreamReader(Request.Body);
                string decodedBodyText = reader.ReadToEnd();
                decodedBody = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(decodedBodyText, MainClass.serializerOptions);
            }
            catch (Exception e)
            {
                Response.StatusCode = 400;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(400, ServerStringResponse.InvalidBody, e)
                }, MainClass.serializerOptions);
            }
            if (decodedBody == null)
            {
                Response.StatusCode = 400;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(400, ServerStringResponse.InvalidBody)
                }, MainClass.serializerOptions);
            }

            ServerConfig.Set(decodedBody);

            return Json(new ObjectResponse<Dictionary<string, Dictionary<string, object>>>()
            {
                Data = ServerConfig.Get(),
                Success = true
            }, MainClass.serializerOptions);
        }

        [HttpPost("reset")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<Dictionary<string, Dictionary<string, object>>>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        [ProducesResponseType(500, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult ResetConfig(string token)
        {
            try
            {
                ServerConfig.Set(ServerConfig.DefaultData);
            }
            catch (Exception e)
            {
                Response.StatusCode = 500;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(500, $"Failed to set config", e)
                }, MainClass.serializerOptions);
            }

            return Json(new ObjectResponse<Dictionary<string, Dictionary<string, object>>>()
            {
                Data = ServerConfig.Get(),
                Success = true
            }, MainClass.serializerOptions);
        }

        [HttpGet("save")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<Dictionary<string, Dictionary<string, object>>>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult Save(string token)
        {
            MainClass.Save();

            return Json(new ObjectResponse<Dictionary<string, Dictionary<string, object>>>()
            {
                Data = ServerConfig.Get(),
                Success = true
            }, MainClass.serializerOptions);
        }
    }
}
