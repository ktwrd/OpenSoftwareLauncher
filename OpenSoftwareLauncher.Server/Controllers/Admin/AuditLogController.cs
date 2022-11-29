using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using OSLCommon;
using OSLCommon.Authorization;
using OSLCommon.Logging;
using System.Linq;

namespace OpenSoftwareLauncher.Server.Controllers.Admin
{
    [Route("admin/auditlog")]
    [ApiController]
    [OSLAuthRequired]
    [OSLAuthPermission(AccountPermission.AUDITLOG_GLOBAL)]
    public class AuditLogController : Controller
    {
        [HttpGet("all")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<AuditLogEntry[]>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult FetchAll(string token)
        {
            var result = MainClass.contentManager.AuditLogManager.GetAll().Result;

            return Json(new ObjectResponse<AuditLogEntry[]>()
            {
                Success = true,
                Data = result
            }, MainClass.serializerOptions);
        }

        [HttpGet("byType")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<AuditLogEntry[]>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult FetchByAction(string token, AuditType auditType)
        {
            var result = MainClass.contentManager.AuditLogManager.GetByType(auditType).Result;

            return Json(new ObjectResponse<AuditLogEntry[]>()
            {
                Success = true,
                Data = result
            }, MainClass.serializerOptions);
        }

        [HttpGet("byUsername")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<AuditLogEntry[]>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult FetchByAccount(string token, string? username = null)
        {
            AuditLogEntry[] result;
            if (username == null)
            {
                result = MainClass.contentManager.AuditLogManager.GetWithFilter(Builders<AuditLogEntry>
                    .Filter
                    .Eq("Username", "")).Result;
            }
            else
            {
                result = MainClass.contentManager.AuditLogManager.GetByUsername(username).Result;
            }

            return Json(new ObjectResponse<AuditLogEntry[]>()
            {
                Success = true,
                Data = result
            }, MainClass.serializerOptions);
        }

        [HttpGet("byRange")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<AuditLogEntry[]>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult FetchByTimestampRange(string token, long min, long max, AuditType auditType = AuditType.Any)
        {
            AuditLogEntry[] result = MainClass.contentManager.AuditLogManager.GetWithFilter(
                Builders<AuditLogEntry>
                    .Filter
                    .Where(v => v.Timestamp > min && v.Timestamp < max)).Result;
            if (auditType != AuditType.Any)
            {
                result = result.Where(v => v.ActionType == auditType).ToArray();
            }

            return Json(new ObjectResponse<AuditLogEntry[]>()
            {
                Success = true,
                Data = result
            }, MainClass.serializerOptions);
        }
    }
}
