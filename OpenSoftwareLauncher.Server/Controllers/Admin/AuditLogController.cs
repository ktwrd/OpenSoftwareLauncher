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
    public class AuditLogController : Controller
    {
        public static AccountPermission[] RequiredPermissions = new AccountPermission[]
        {
            AccountPermission.AUDITLOG_GLOBAL
        };

        [HttpGet("all")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<AuditLogEntry[]>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult FetchAll(string token)
        {
            var authRes = MainClass.ValidatePermissions(token, RequiredPermissions);
            if (authRes != null)
            {
                Response.StatusCode = authRes?.Data.Code ?? 0;
                return Json(authRes, MainClass.serializerOptions);
            }

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
            var authRes = MainClass.ValidatePermissions(token, RequiredPermissions);
            if (authRes != null)
            {
                Response.StatusCode = authRes?.Data.Code ?? 0;
                return Json(authRes, MainClass.serializerOptions);
            }

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
            var authRes = MainClass.ValidatePermissions(token, RequiredPermissions);
            if (authRes != null)
            {
                Response.StatusCode = authRes?.Data.Code ?? 0;
                return Json(authRes, MainClass.serializerOptions);
            }

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
            var authRes = MainClass.ValidatePermissions(token, RequiredPermissions);
            if (authRes != null)
            {
                Response.StatusCode = authRes?.Data.Code ?? 0;
                return Json(authRes, MainClass.serializerOptions);
            }

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
