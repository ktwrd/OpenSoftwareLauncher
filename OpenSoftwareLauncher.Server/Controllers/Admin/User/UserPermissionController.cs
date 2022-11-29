using OSLCommon;
using OSLCommon.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OSLCommon.Logging;

namespace OpenSoftwareLauncher.Server.Controllers.Admin.User
{
    [Route("admin/user/permission")]
    [ApiController]
    [OSLAuthRequired]
    [OSLAuthPermission(AccountPermission.USER_PERMISSION_MODIFY)]
    public class UserPermissionController : Controller
    {
        [HttpGet("list")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<AccountPermission[]>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        [ProducesResponseType(404, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult List(string token, string username)
        {
            var account = MainClass.contentManager.AccountManager.GetAccountByUsername(username);
            if (account == null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(StatusCodes.Status404NotFound, ServerStringResponse.AccountNotFound)
                }, MainClass.serializerOptions);
            }

            return Json(new ObjectResponse<AccountPermission[]>()
            {
                Success = true,
                Data = account.Permissions
            }, MainClass.serializerOptions);
        }

        [HttpGet("grant")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<bool>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        [ProducesResponseType(404, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult Grant(string token, string username, AccountPermission permission)
        {
            var tokenAccount = MainClass.contentManager.AccountManager.GetAccount(token);

            var account = MainClass.contentManager.AccountManager.GetAccountByUsername(username);
            if (account == null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(StatusCodes.Status404NotFound, ServerStringResponse.AccountNotFound)
                }, MainClass.serializerOptions);
            }

            MainClass.contentManager.AuditLogManager.Create(new AccountPermissionGrantEntryData(account, permission, false), tokenAccount).Wait();

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
            var tokenAccount = MainClass.contentManager.AccountManager.GetAccount(token);

            var account = MainClass.contentManager.AccountManager.GetAccountByUsername(username);
            if (account == null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(StatusCodes.Status404NotFound, ServerStringResponse.AccountNotFound)
                }, MainClass.serializerOptions);
            }

            MainClass.contentManager.AuditLogManager.Create(new AccountPermissionRevokeEntryData(account, permission), tokenAccount).Wait();

            return Json(new ObjectResponse<bool>()
            {
                Success = true,
                Data = account.RevokePermission(permission)
            }, MainClass.serializerOptions);
        }
    }
}
