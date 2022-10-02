using OSLCommon;
using OSLCommon.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;

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
            var item = MainClass.contentManager.SystemAnnouncement.GetLatest();
            var list = new List<SystemAnnouncementEntry>();
            if (item != null && MainClass.contentManager.SystemAnnouncement.Active)
                list.Add(item);
            return Json(new ObjectResponse<SystemAnnouncementEntry[]>()
            {
                Success = true,
                Data = list.ToArray()
            }, MainClass.serializerOptions);
        }

        [HttpGet("new")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<SystemAnnouncementSummary>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<string>))]
        public ActionResult Set(string token, string content, bool? active=true)
        {
            if (!MainClass.contentManager.AccountManager.AccountHasPermission(token, AccountPermission.ANNOUNCEMENT_MANAGE))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new ObjectResponse<string>()
                {
                    Success = false,
                    Data = ServerStringResponse.InvalidCredential
                }, MainClass.serializerOptions);
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
        [ProducesResponseType(401, Type = typeof(ObjectResponse<string>))]
        public ActionResult UpdateActiveStatus(string token, bool active)
        {
            if (!MainClass.contentManager.AccountManager.AccountHasPermission(token, AccountPermission.ANNOUNCEMENT_MANAGE))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new ObjectResponse<string>()
                {
                    Success = false,
                    Data = "Invalid Permissions"
                }, MainClass.serializerOptions);
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
        [ProducesResponseType(401, Type = typeof(ObjectResponse<string>))]
        public ActionResult FetchAll(string token)
        {
            if (!MainClass.contentManager.AccountManager.AccountHasPermission(token, AccountPermission.ANNOUNCEMENT_MANAGE))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new ObjectResponse<string>()
                {
                    Success = false,
                    Data = "Invalid Permissions"
                }, MainClass.serializerOptions);
            }
            return Json(new ObjectResponse<SystemAnnouncementEntry[]>()
            {
                Success = true,
                Data = MainClass.contentManager.SystemAnnouncement.Entries.ToArray()
            }, MainClass.serializerOptions);
        }

        [HttpGet("summary")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<SystemAnnouncementSummary>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<string>))]
        public ActionResult GetSummary(string token)
        {
            if (!MainClass.contentManager.AccountManager.AccountHasPermission(token, AccountPermission.ANNOUNCEMENT_MANAGE))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new ObjectResponse<string>()
                {
                    Success = false,
                    Data = "Invalid Permissions"
                }, MainClass.serializerOptions);
            }
            return Json(new ObjectResponse<SystemAnnouncementSummary>()
            {
                Success = true,
                Data = MainClass.contentManager.SystemAnnouncement.GetSummary()
            }, MainClass.serializerOptions);
        }

    
        [HttpGet("setData")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<SystemAnnouncementSummary>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<string>))]
        public ActionResult SetData(string token, string content)
        {
            if (!MainClass.contentManager.AccountManager.AccountHasPermission(token, AccountPermission.ADMINISTRATOR))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new ObjectResponse<string>()
                {
                    Success = false,
                    Data = "Invalid Permissions"
                }, MainClass.serializerOptions);
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

            MainClass.contentManager.SystemAnnouncement.Read(WebUtility.UrlDecode(content));
            MainClass.contentManager.SystemAnnouncement.OnUpdate();

            return Json(new ObjectResponse<SystemAnnouncementSummary>()
            {
                Success = true,
                Data = attemptedDeserialized
            }, MainClass.serializerOptions);
        }
    }
}
