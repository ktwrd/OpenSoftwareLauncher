using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OSLCommon;
using OSLCommon.Authorization;
using OSLCommon.Logging;
using System.Linq;

namespace OpenSoftwareLauncher.Server.Controllers.Admin.User
{
    [Route("admin/user/license")]
    [ApiController]
    [OSLAuthRequired]
    [OSLAuthPermission(AccountPermission.USER_LICENSE_MODIFY)]
    [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
    [ProducesResponseType(404, Type = typeof(ObjectResponse<HttpException>))]
    [ProducesResponseType(409, Type = typeof(ObjectResponse<HttpException>))]
    public class UserLicenseController : Controller
    {
        [HttpGet("grant")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<string[]>))]
        public ActionResult Grant(string token, string username, string license)
        {
            var tokenAccount = MainClass.contentManager.AccountManager.GetAccount(token, false);

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
            string[] previous = account.Licenses.ToArray();
            bool didItWork = account.GrantLicense(license);
            if (!didItWork)
            {
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(StatusCodes.Status409Conflict, ServerStringResponse.LicenseExists)
                }, MainClass.serializerOptions);
            }

            MainClass.contentManager.AuditLogManager.Create(new AccountLicenseUpdateEntryData(account, previous, account.Licenses.ToArray()), tokenAccount).Wait();

            return Json(new ObjectResponse<string[]>()
            {
                Success = true,
                Data = account.Licenses
            }, MainClass.serializerOptions);
        }

        [HttpGet("revoke")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<string[]>))]
        public ActionResult Revoke(string token, string username, string license)
        {
            var tokenAccount = MainClass.contentManager.AccountManager.GetAccount(token, false);

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

            string[] previous = account.Licenses.ToArray();
            bool didItWork = account.RevokeLicense(license);
            if (!didItWork)
            {
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(StatusCodes.Status409Conflict, ServerStringResponse.LicenseNotFound)
                }, MainClass.serializerOptions);
            }
            MainClass.contentManager.AuditLogManager.Create(new AccountLicenseUpdateEntryData(account, previous, account.Licenses.ToArray()), tokenAccount).Wait();
            return Json(new ObjectResponse<string[]>()
            {
                Success = true,
                Data = account.Licenses
            }, MainClass.serializerOptions);
        }
    }
}
