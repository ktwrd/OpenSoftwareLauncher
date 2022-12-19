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
        [HttpGet("latest")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<SystemAnnouncementEntry[]>))]
        public ActionResult Fetch()
        {
            var item = MainClass.ContentManager.SystemAnnouncement.GetLatest();
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
        [OSLAuthRequired]
        [OSLAuthPermission(AccountPermission.ANNOUNCEMENT_MANAGE)]
        public ActionResult Set(string token, string content, bool? active=true)
        {
            var account = MainClass.ContentManager.AccountManager.GetAccount(token);
            var announcement = MainClass.ContentManager.SystemAnnouncement.Set(content, active ?? true);
            MainClass.ContentManager.AuditLogManager.Create(new AnnouncementCreateEntryData(announcement), account).Wait();

            return Json(new ObjectResponse<SystemAnnouncementSummary>()
            {
                Success = true,
                Data = MainClass.ContentManager.SystemAnnouncement.GetSummary()
            }, MainClass.serializerOptions);
        }

        [HttpGet("update")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<object?>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        [OSLAuthRequired]
        [OSLAuthPermission(AccountPermission.ANNOUNCEMENT_MANAGE)]
        public ActionResult UpdateActiveStatus(string token, bool active)
        {
            var account = MainClass.ContentManager.AccountManager.GetAccount(token);

            MainClass.ContentManager.SystemAnnouncement.Active = active;
            MainClass.ContentManager.SystemAnnouncement.OnUpdate();
            MainClass.ContentManager.AuditLogManager.Create(new AnnouncementStateToggleEntryData
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
        [OSLAuthRequired]
        [OSLAuthPermission(AccountPermission.ANNOUNCEMENT_MANAGE)]
        public ActionResult FetchAll(string token)
        {
            return Json(new ObjectResponse<SystemAnnouncementEntry[]>()
            {
                Success = true,
                Data = MainClass.ContentManager.SystemAnnouncement.Entries.ToArray()
            }, MainClass.serializerOptions);
        }

        [HttpGet("summary")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<SystemAnnouncementSummary>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        [OSLAuthRequired]
        [OSLAuthPermission(AccountPermission.ANNOUNCEMENT_MANAGE)]
        public ActionResult GetSummary(string token)
        {
            return Json(new ObjectResponse<SystemAnnouncementSummary>()
            {
                Success = true,
                Data = MainClass.ContentManager.SystemAnnouncement.GetSummary()
            }, MainClass.serializerOptions);
        }

    
        [HttpGet("setData")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<SystemAnnouncementSummary>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        [OSLAuthRequired]
        [OSLAuthPermission(AccountPermission.ANNOUNCEMENT_MANAGE)]
        public ActionResult SetData(string token, string content)
        {
            var account = MainClass.ContentManager.AccountManager.GetAccount(token);

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
            var prevIdList = MainClass.ContentManager.SystemAnnouncement.GetAll().Select(v => v.ID).ToArray();
            foreach (var item in attemptedDeserialized.Entries)
            {
                if (!prevIdList.Contains(item.ID))
                {
                    MainClass.ContentManager.AuditLogManager.Create(new AnnouncementCreateEntryData(item), account).Wait();
                }
                else
                {
                    var current = MainClass.ContentManager.SystemAnnouncement.GetAll().Where(v => v.ID == item.ID).FirstOrDefault();
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
                                MainClass.ContentManager.AuditLogManager.Create(new AnnouncementModifyEntryData(current, item), account).Wait();
                            }
                        }
                    }
                }
            }
            foreach (var item in attemptedDeserialized.Entries)
            {
                MainClass.ContentManager.SystemAnnouncement.Set(item.ID, item);
            }
            foreach (var item in MainClass.ContentManager.SystemAnnouncement.GetAll().Where(v => !idList.Contains(v.ID)))
            {
                MainClass.ContentManager.SystemAnnouncement.RemoveId(item.ID);
                MainClass.ContentManager.AuditLogManager.Create(new AnnouncementDeleteEntryData(item), account).Wait();
            }

            if (attemptedDeserialized.Active != MainClass.ContentManager.SystemAnnouncement.Active)
            {
                MainClass.ContentManager.AuditLogManager.Create(new AnnouncementStateToggleEntryData
                {
                    State = attemptedDeserialized.Active
                }, account).Wait();
                MainClass.ContentManager.SystemAnnouncement.Active = attemptedDeserialized.Active;
                MainClass.ContentManager.SystemAnnouncement.OnUpdate();
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
        [OSLAuthRequired]
        [OSLAuthPermission(AccountPermission.ANNOUNCEMENT_MANAGE)]
        public ActionResult RemoveAnnouncement(string token, string id)
        {
            var account = MainClass.ContentManager.AccountManager.GetAccount(token);

            bool exists = MainClass.ContentManager.SystemAnnouncement.GetAll().Where(v => v.ID == id).Count() > 0;
            if (exists)
            {
                var item = MainClass.ContentManager.SystemAnnouncement.GetAll().Where(v => v.ID == id).FirstOrDefault();
                MainClass.ContentManager.AuditLogManager.Create(new AnnouncementDeleteEntryData(item), account).Wait();
                MainClass.ContentManager.SystemAnnouncement.RemoveId(id);
            }

            return Json(new ObjectResponse<SystemAnnouncementSummary>()
            {
                Success = true,
                Data = MainClass.ContentManager.SystemAnnouncement.GetSummary()
            }, MainClass.serializerOptions);
        }
    }
}
