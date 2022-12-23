using OSLCommon;
using OSLCommon.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace OpenSoftwareLauncher.Server.Controllers.Account
{
    [Route("account/[controller]")]
    [ApiController]
    [OSLAuthRequired]
    public class PermissionsController : Controller
    {
        [HttpGet]
        public ActionResult Index(string token)
        {
            var account = MainClass.GetService<MongoAccountManager>()?.GetAccount(token, true);

            return Json(new ObjectResponse<AccountPermission[]>()
            {
                Success = true,
                Data = account?.Permissions ?? Array.Empty<AccountPermission>()
            }, MainClass.serializerOptions);
        }
    }
}
