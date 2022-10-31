using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OSLCommon;
using OSLCommon.Authorization;

namespace OpenSoftwareLauncher.Server.Controllers.Admin.User
{
    [Route("admin/user/license")]
    [ApiController]
    public class UserLicenseController : Controller
    {
        public static AccountPermission[] RequiredPermissions = new AccountPermission[]
        {
            AccountPermission.USER_LICENSE_MODIFY
        };
        [HttpGet("grant")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<string[]>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        [ProducesResponseType(404, Type = typeof(ObjectResponse<HttpException>))]
        [ProducesResponseType(409, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult Grant(string token, string username, string license)
        {
            var authRes = MainClass.ValidatePermissions(token, RequiredPermissions);
            if (authRes != null)
            {
                Response.StatusCode = authRes?.Data.Code ?? 0;
                return Json(authRes, MainClass.serializerOptions);
            }

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

            bool didItWork = account.GrantLicense(license);
            if (!didItWork)
            {
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(StatusCodes.Status409Conflict, ServerStringResponse.LicenseExists)
                });
            }
            return Json(new ObjectResponse<string[]>()
            {
                Success = true,
                Data = account.Licenses
            }, MainClass.serializerOptions);
        }

        [HttpGet("revoke")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<string[]>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        [ProducesResponseType(404, Type = typeof(ObjectResponse<HttpException>))]
        [ProducesResponseType(409, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult Revoke(string token, string username, string license)
        {
            var authRes = MainClass.ValidatePermissions(token, RequiredPermissions);
            if (authRes != null)
            {
                Response.StatusCode = authRes?.Data.Code ?? 0;
                return Json(authRes, MainClass.serializerOptions);
            }

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

            bool didItWork = account.RevokeLicense(license);
            if (!didItWork)
            {
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(StatusCodes.Status409Conflict, ServerStringResponse.LicenseNotFound)
                });
            }
            return Json(new ObjectResponse<string[]>()
            {
                Success = true,
                Data = account.Licenses
            }, MainClass.serializerOptions);
        }
    }
}
