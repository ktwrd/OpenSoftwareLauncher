using OSLCommon;
using OSLCommon.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace OpenSoftwareLauncher.Server.Controllers.Admin.User
{
    [Route("admin/user/group")]
    [ApiController]
    [OSLAuthRequired]
    [OSLAuthPermission(AccountPermission.USER_GROUP_MODIFY)]
    [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
    public class UserGroupController : Controller
    {
        [HttpGet("list")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<string[]>))]
        [ProducesResponseType(404, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult List(string token, string username)
        {
            var targetAccount = MainClass.GetService<MongoAccountManager>()?.GetAccountByUsername(username);
            if (targetAccount == null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(StatusCodes.Status404NotFound, ServerStringResponse.AccountNotFound)
                }, MainClass.serializerOptions);
            }

            return Json(new ObjectResponse<string[]>()
            {
                Success = true,
                Data = targetAccount.Groups
            }, MainClass.serializerOptions);
        }

        [HttpPost("set")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<object>))]
        [ProducesResponseType(500, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult SetContent(string token)
        {
            var syncIOFeature = HttpContext.Features.Get<IHttpBodyControlFeature>();
            if (syncIOFeature != null)
            {
                syncIOFeature.AllowSynchronousIO = true;
            }
            ObjectResponse<Dictionary<string, string[]>>? decodedBody = null;
            try
            {
                StreamReader reader = new StreamReader(Request.Body);
                string decodedBodyText = reader.ReadToEnd();
                decodedBody = JsonSerializer.Deserialize<ObjectResponse<Dictionary<string, string[]>>>(decodedBodyText, MainClass.serializerOptions);
            }
            catch (Exception e)
            {
                Response.StatusCode = 401;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(401, "Invalid Body", e)
                }, MainClass.serializerOptions);
            }
            if (decodedBody == null)
            {
                Response.StatusCode = 401;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(401, "Invalid Body")
                }, MainClass.serializerOptions);
            }

            try
            {            
                MainClass.GetService<MongoAccountManager>()?.SetUserGroups(decodedBody.Data);
            }
            catch (Exception except)
            {
                Response.StatusCode = 500;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(500, $"Exception when invoking MainClass.GetService<MongoAccountManager>()?.SetUserGroups", except)
                }, MainClass.serializerOptions);
            }

            return Json(new ObjectResponse<object?>()
            {
                Success = true,
                Data = null
            }, MainClass.serializerOptions);
        }

        [HttpGet("grant")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<bool>))]
        [ProducesResponseType(404, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult Grant(string token, string username, string group)
        {
            var account = MainClass.GetService<MongoAccountManager>()?.GetAccountByUsername(username);
            if (account == null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(StatusCodes.Status404NotFound, ServerStringResponse.AccountNotFound)
                }, MainClass.serializerOptions);
            }

            return Json(new ObjectResponse<bool>()
            {
                Success = true,
                Data = account.AddGroup(group)
            }, MainClass.serializerOptions);
        }

        [HttpGet("revoke")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<object>))]
        [ProducesResponseType(404, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult RevokeGroup(string token, string username, string group)
        {
            var account = MainClass.GetService<MongoAccountManager>()?.GetAccountByUsername(username);
            if (account == null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(StatusCodes.Status404NotFound, ServerStringResponse.AccountNotFound)
                }, MainClass.serializerOptions);
            }

            return Json(new ObjectResponse<bool>()
            {
                Success = true,
                Data = account.RevokeGroup(group)
            }, MainClass.serializerOptions);
        }
    }
}
