using OSLCommon;
using OSLCommon.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Linq;
using OSLCommon.Logging;
using JsonDiffPatchDotNet;

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
            var account = MainClass.contentManager.AccountManager.GetAccount(token);
            var announcement = MainClass.contentManager.SystemAnnouncement.Set(content, active ?? true);
            MainClass.contentManager.AuditLogManager.Create(new AnnouncementCreateEntryData(announcement), account).Wait();

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
            var account = MainClass.contentManager.AccountManager.GetAccount(token);

            MainClass.contentManager.SystemAnnouncement.Active = active;
            MainClass.contentManager.SystemAnnouncement.OnUpdate();
            MainClass.contentManager.AuditLogManager.Create(new AnnouncementStateToggleEntryData
            {
                State = active
            }, account).Wait();
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
            var account = MainClass.contentManager.AccountManager.GetAccount(token);

            var attemptedDeserialized = JsonSerializer.Deserialize<SystemAnnouncementSummary>(content, MainClass.serializerOptions);
            if (attemptedDeserialized == null) {
                Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                return Json(new ObjectResponse<string>()
                {
                    Success = false,
                    Data = "Attempted to deserialized, but it failed (returned null)"
                }, MainClass.serializerOptions);
            }

            var idList = attemptedDeserialized.Entries.ToList().Select(v => v.ID).ToList();
            var prevIdList = MainClass.contentManager.SystemAnnouncement.GetAll().Select(v => v.ID).ToArray();
            foreach (var item in attemptedDeserialized.Entries)
            {
                if (!prevIdList.Contains(item.ID))
                {
                    MainClass.contentManager.AuditLogManager.Create(new AnnouncementCreateEntryData(item), account).Wait();
                }
                else
                {
                    var current = MainClass.contentManager.SystemAnnouncement.GetAll().Where(v => v.ID == item.ID).FirstOrDefault();
                    if (current != null)
                    {
                        var diff = (new JsonDiffPatch()).Diff(
                            JsonSerializer.Serialize(current, MainClass.serializerOptions),
                            JsonSerializer.Serialize(item, MainClass.serializerOptions));
                        if (diff != null)
                        {
                            diff = diff.ReplaceLineEndings("");
                            var count = JsonSerializer.Deserialize<Dictionary<string, dynamic[]>>(diff, MainClass.serializerOptions);
                            if (count.Count > 0)
                            {
                                MainClass.contentManager.AuditLogManager.Create(new AnnouncementModifyEntryData(current, item), account).Wait();
                            }
                        }
                    }
                }
            }
            foreach (var item in attemptedDeserialized.Entries)
            {
                MainClass.contentManager.SystemAnnouncement.Set(item.ID, item);
            }
            foreach (var item in MainClass.contentManager.SystemAnnouncement.GetAll().Where(v => !idList.Contains(v.ID)))
            {
                MainClass.contentManager.SystemAnnouncement.RemoveId(item.ID);
                MainClass.contentManager.AuditLogManager.Create(new AnnouncementDeleteEntryData(item), account).Wait();
            }

            if (attemptedDeserialized.Active != MainClass.contentManager.SystemAnnouncement.Active)
            {
                MainClass.contentManager.AuditLogManager.Create(new AnnouncementStateToggleEntryData
                {
                    State = attemptedDeserialized.Active
                }, account).Wait();
                MainClass.contentManager.SystemAnnouncement.Active = attemptedDeserialized.Active;
                MainClass.contentManager.SystemAnnouncement.OnUpdate();
            }

            return Json(new ObjectResponse<SystemAnnouncementSummary>()
            {
                Success = true,
                Data = attemptedDeserialized
            }, MainClass.serializerOptions);
        }

        [HttpGet("remove")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<SystemAnnouncementSummary>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult RemoveAnnouncement(string token, string id)
        {
            var authRes = MainClass.ValidatePermissions(token, RequiredPermissions);
            if (authRes != null)
            {
                Response.StatusCode = authRes?.Data.Code ?? 0;
                return Json(authRes, MainClass.serializerOptions);
            }
            var account = MainClass.contentManager.AccountManager.GetAccount(token);

            bool exists = MainClass.contentManager.SystemAnnouncement.GetAll().Where(v => v.ID == id).Count() > 0;
            if (exists)
            {
                var item = MainClass.contentManager.SystemAnnouncement.GetAll().Where(v => v.ID == id).FirstOrDefault();
                MainClass.contentManager.AuditLogManager.Create(new AnnouncementDeleteEntryData(item), account).Wait();
                MainClass.contentManager.SystemAnnouncement.RemoveId(id);
            }

            return Json(new ObjectResponse<SystemAnnouncementSummary>()
            {
                Success = true,
                Data = MainClass.contentManager.SystemAnnouncement.GetSummary()
            }, MainClass.serializerOptions);
        }
    }
}
