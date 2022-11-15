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
        public ActionResult FetchAll(string token)
        {
            var authRes = MainClass.ValidatePermissions(token, AccountPermission.AUDITLOG_SELF);
            if (authRes != null)
            {
                Response.StatusCode = authRes?.Data.Code ?? 0;
                return Json(authRes, MainClass.serializerOptions);
            }

            var account = MainClass.contentManager.AccountManager.GetAccount(token);

            var result = MainClass.contentManager.AuditLogManager.GetByUsername(account.Username).Result;

            return Json(new ObjectResponse<AuditLogEntry[]>()
            {
                Success = true,
                Data = result
            }, MainClass.serializerOptions);
        }
    }
}
