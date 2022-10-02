using OSLCommon;
using OSLCommon.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace OpenSoftwareLauncher.Server.Controllers.Account
{
    [Route("account/[controller]")]
    [ApiController]
    public class PermissionsController : Controller
    {
        [HttpGet]
        public ActionResult Index(string token)
        {
            var account = MainClass.contentManager.AccountManager.GetAccount(token ?? "", true);
            if (account == null)
            {
                Response.StatusCode = 401;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(401, ServerStringResponse.InvalidCredential)
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

            return Json(new ObjectResponse<AccountPermission[]>()
            {
                Success = true,
                Data = account.Permissions.ToArray()
            }, MainClass.serializerOptions);
        }
    }
}
