using OSLCommon;
using OSLCommon.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Linq;

namespace OpenSoftwareLauncher.Server.Controllers.Admin
{
    [Route("admin/announcement")]
    [ApiController]
    public class SystemAnnouncementController : Controller
    {
        public static AccountPermission[] RequiredPermissions = new AccountPermission[]
        {
            AccountPermission.ANNOUNCEMENT_MANAGE
        };
        [HttpGet("latest")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<SystemAnnouncementEntry[]>))]
        public ActionResult Fetch()
        {
            var item = MainClass.contentManager.SystemAnnouncement.GetLatest();
            var list = new List<SystemAnnouncementEntry>();
            if (item != null)
                list.Add(item);
            return Json(new ObjectResponse<SystemAnnouncementEntry[]>()
            {
                Success = true,
                Data = list.ToArray()
            }, MainClass.serializerOptions);
        }

        [HttpGet("new")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<SystemAnnouncementSummary>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult Set(string token, string content, bool? active=true)
        {
            var authRes = MainClass.ValidatePermissions(token, RequiredPermissions);
            if (authRes != null)
            {
                Response.StatusCode = authRes?.Data.Code ?? 0;
                return Json(authRes, MainClass.serializerOptions);
            }

            MainClass.contentManager.SystemAnnouncement.Set(content, active ?? true);
            return Json(new ObjectResponse<SystemAnnouncementSummary>()
            {
                Success = true,
                Data = MainClass.contentManager.SystemAnnouncement.GetSummary()
            }, MainClass.serializerOptions);
        }

        [HttpGet("update")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<object?>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult UpdateActiveStatus(string token, bool active)
        {
            var authRes = MainClass.ValidatePermissions(token, RequiredPermissions);
            if (authRes != null)
            {
                Response.StatusCode = authRes?.Data.Code ?? 0;
                return Json(authRes, MainClass.serializerOptions);
            }

            MainClass.contentManager.SystemAnnouncement.Active = active;
            MainClass.contentManager.SystemAnnouncement.OnUpdate();
            return Json(new ObjectResponse<object>()
            {
                Success = true,
                Data = null
            }, MainClass.serializerOptions);
        }

        [HttpGet("all")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<SystemAnnouncementEntry[]>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult FetchAll(string token)
        {
            var authRes = MainClass.ValidatePermissions(token, RequiredPermissions);
            if (authRes != null)
            {
                Response.StatusCode = authRes?.Data.Code ?? 0;
                return Json(authRes, MainClass.serializerOptions);
            }
            return Json(new ObjectResponse<SystemAnnouncementEntry[]>()
            {
                Success = true,
                Data = MainClass.contentManager.SystemAnnouncement.Entries.ToArray()
            }, MainClass.serializerOptions);
        }

        [HttpGet("summary")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<SystemAnnouncementSummary>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult GetSummary(string token)
        {
            var authRes = MainClass.ValidatePermissions(token, RequiredPermissions);
            if (authRes != null)
            {
                Response.StatusCode = authRes?.Data.Code ?? 0;
                return Json(authRes, MainClass.serializerOptions);
            }
            return Json(new ObjectResponse<SystemAnnouncementSummary>()
            {
                Success = true,
                Data = MainClass.contentManager.SystemAnnouncement.GetSummary()
            }, MainClass.serializerOptions);
        }

    
        [HttpGet("setData")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<SystemAnnouncementSummary>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult SetData(string token, string content)
        {
            var authRes = MainClass.ValidatePermissions(token, RequiredPermissions);
            if (authRes != null)
            {
                Response.StatusCode = authRes?.Data.Code ?? 0;
                return Json(authRes, MainClass.serializerOptions);
            }

            var attemptedDeserialized = JsonSerializer.Deserialize<SystemAnnouncementSummary>(content, MainClass.serializerOptions);
            if (attemptedDeserialized == null) {
                Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                return Json(new ObjectResponse<string>()
                {
                    Success = false,
                    Data = "Attempted to deserialized, but it failed (returned null)"
                }, MainClass.serializerOptions);
            }

            var idList = new List<string>();
            foreach (var item in attemptedDeserialized.Entries)
            {
                MainClass.contentManager.SystemAnnouncement.Set(item.ID, item);
                idList.Add(item.ID);
            }
            foreach (var item in MainClass.contentManager.SystemAnnouncement.GetAll().Where(v => !idList.Contains(v.ID)))
            {
                MainClass.contentManager.SystemAnnouncement.RemoveId(item.ID);
            }

            return Json(new ObjectResponse<SystemAnnouncementSummary>()
            {
                Success = true,
                Data = attemptedDeserialized
            }, MainClass.serializerOptions);
        }
    }
}
