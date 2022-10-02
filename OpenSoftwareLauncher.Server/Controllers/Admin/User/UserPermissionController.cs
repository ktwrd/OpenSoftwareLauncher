using OSLCommon;
using OSLCommon.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Security;

namespace OpenSoftwareLauncher.Server.Controllers.Admin.User
{
    [Route("admin/user/permission")]
    [ApiController]
    public class UserPermissionController : Controller
    {
        public static AccountPermission[] RequiredPermissions = new AccountPermission[]
        {
            AccountPermission.USER_PERMISSION_MODIFY
        };

        [HttpGet("list")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<AccountPermission[]>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        [ProducesResponseType(404, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult List(string token, string username)
        {
            if (!MainClass.contentManager.AccountManager.AccountHasPermission(token, RequiredPermissions))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(StatusCodes.Status401Unauthorized, ServerStringResponse.InvalidCredential)
                }, MainClass.serializerOptions);
            }

            var account = MainClass.contentManager.AccountManager.GetAccountByUsername(username);
            if (account == null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(StatusCodes.Status404NotFound, ServerStringResponse.AccountNotFound(username))
                }, MainClass.serializerOptions);
            }

            return Json(new ObjectResponse<AccountPermission[]>()
            {
                Success = true,
                Data = account.Permissions.ToArray()
            }, MainClass.serializerOptions);
        }

        [HttpGet("grant")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<bool>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        [ProducesResponseType(404, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult Grant(string token, string username, AccountPermission permission)
        {
            if (!MainClass.contentManager.AccountManager.AccountHasPermission(token, RequiredPermissions))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(StatusCodes.Status401Unauthorized, ServerStringResponse.InvalidCredential)
                }, MainClass.serializerOptions);
            }

            var account = MainClass.contentManager.AccountManager.GetAccountByUsername(username);
            if (account == null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(StatusCodes.Status404NotFound, ServerStringResponse.AccountNotFound(username))
                }, MainClass.serializerOptions);
            }

            return Json(new ObjectResponse<bool>()
            {
                Success = true,
                Data = account.GrantPermission(permission)
            }, MainClass.serializerOptions);
        }

        [HttpGet("revoke")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<bool>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        [ProducesResponseType(404, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult Revoke(string token, string username, AccountPermission permission)
        {
            if (!MainClass.contentManager.AccountManager.AccountHasPermission(token, RequiredPermissions))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(StatusCodes.Status401Unauthorized, ServerStringResponse.InvalidCredential)
                }, MainClass.serializerOptions);
            }

            var account = MainClass.contentManager.AccountManager.GetAccountByUsername(username);
            if (account == null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(StatusCodes.Status404NotFound, ServerStringResponse.AccountNotFound(username))
                }, MainClass.serializerOptions);
            }

            return Json(new ObjectResponse<bool>()
            {
                Success = true,
                Data = account.RevokePermission(permission)
            }, MainClass.serializerOptions);
        }
    }
}
