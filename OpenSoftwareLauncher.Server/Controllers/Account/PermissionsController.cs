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
            var authRes = MainClass.Validate(token);
            if (authRes != null)
            {
                Response.StatusCode = authRes?.Data.Code ?? 0;
                return Json(authRes, MainClass.serializerOptions);
            }
            var account = MainClass.contentManager.AccountManager.GetAccount(token, true);

            return Json(new ObjectResponse<AccountPermission[]>()
            {
                Success = true,
                Data = account.Permissions
            }, MainClass.serializerOptions);
        }
    }
}
