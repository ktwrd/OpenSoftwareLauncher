using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OSLCommon;
using OSLCommon.Authorization;
using OSLCommon.Logging;

namespace OpenSoftwareLauncher.Server.Controllers
{
    [Route("auditLog")]
    [ApiController]
    public class AuditLogController : Controller
    {
        [HttpGet("fetch")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<AuditLogEntry[]>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        [OSLAuthPermission(AccountPermission.AUDITLOG_SELF)]
        [OSLAuthRequired]
        public ActionResult FetchAll(string token)
        {
            var account = MainClass.ContentManager.AccountManager.GetAccount(token);

            var result = MainClass.ContentManager.AuditLogManager.GetByUsername(account.Username).Result;

            return Json(new ObjectResponse<AuditLogEntry[]>()
            {
                Success = true,
                Data = result
            }, MainClass.serializerOptions);
        }
    }
}
