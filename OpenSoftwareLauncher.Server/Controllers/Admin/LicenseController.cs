using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using OSLCommon;
using OSLCommon.Authorization;
using System.IO;
using System;
using System.Text.Json;
using Licensing = OSLCommon.Licensing;
using OSLCommon.Licensing;
using System.Linq;
using static OSLCommon.Licensing.AccountLicenseManager;
using System.Collections.Generic;
using System.Threading.Tasks;
using OSLCommon.Logging;

namespace OpenSoftwareLauncher.Server.Controllers.Admin
{
    [Route("admin/license")]
    [ApiController]
    [OSLAuthRequired]
    [OSLAuthPermission(AccountPermission.LICENSE_MANAGE)]
    public class LicenseController : Controller
    {
        [HttpPost("generateProductKey")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<CreateLicenseKeyResponse>))]
        [ProducesResponseType(400, Type = typeof(ObjectResponse<HttpException>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult CreateProductKey(string token)
        {
            var account = MainClass.contentManager.AccountManager.GetAccount(token);


            var syncIOFeature = HttpContext.Features.Get<IHttpBodyControlFeature>();
            if (syncIOFeature != null)
            {
                syncIOFeature.AllowSynchronousIO = true;
            }
            Licensing.CreateProductKeyRequest decodedBody;
            try
            {
                StreamReader reader = new StreamReader(Request.Body);
                string decodedBodyText = reader.ReadToEnd();
                decodedBody = JsonSerializer.Deserialize<Licensing.CreateProductKeyRequest>(decodedBodyText, MainClass.serializerOptions);
            }
            catch (Exception e)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(StatusCodes.Status400BadRequest, ServerStringResponse.InvalidBody, e)
                }, MainClass.serializerOptions);
            }
            if (decodedBody == null)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(StatusCodes.Status400BadRequest, ServerStringResponse.InvalidBody)
                }, MainClass.serializerOptions);
            }

            var res = MainClass.contentManager.AccountLicenseManager.CreateLicenseKeys(
                account.Username,
                decodedBody.RemoteLocations,
                decodedBody.Count,
                decodedBody.Permissions,
                decodedBody.Note,
                decodedBody.ExpiryTimestamp,
                decodedBody.GroupLabel).Result;

            var taskList = new List<Task>();
            foreach (var item in res.Keys)
            {
                taskList.Add(new Task(delegate
                {
                    MainClass.contentManager.AuditLogManager.Create(new LicenseCreateEntryData(item), account).Wait();
                }));
            }
            foreach (var i in taskList)
                i.Start();
            Task.WhenAll(taskList);

            return Json(new ObjectResponse<CreateLicenseKeyResponse>
            {
                Success = true,
                Data = res
            }, MainClass.serializerOptions);
        }

        [HttpGet("getKeys")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<LicenseKeyMetadata[]>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult GetLicenseKeys(string token)
        {
            return Json(new ObjectResponse<LicenseKeyMetadata[]>
            {
                Success = true,
                Data = MainClass.contentManager.AccountLicenseManager.GetLicenseKeys().Result
            }, MainClass.serializerOptions);
        }

        [HttpGet("getProductKeys")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<LicenseKeyMetadata[]>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult GetProductLicenseKeys(string token, string remoteLocation)
        {
            var keys = MainClass.contentManager.AccountLicenseManager.GetLicenseKeys().Result
                .Where(v => v.Products.Contains(remoteLocation))
                .ToArray();

            return Json(new ObjectResponse<LicenseKeyMetadata[]>
            {
                Success = true,
                Data = keys
            }, MainClass.serializerOptions);
        }

        /*[HttpGet("getGroup")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<LicenseKeyMetadata[]>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult GetLicenseKeysByGroup(string token, string group)
        {
            
        }*/

        [HttpGet("disableKey")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<LicenseKeyActionResult>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult DisableKey(string token, string keyId)
        {
            LicenseKeyActionResult response = MainClass.contentManager.AccountLicenseManager.DisableLicenseKey(keyId).Result;

            var account = MainClass.contentManager.AccountManager.GetAccount(token);
            if (response == LicenseKeyActionResult.Success)
            {
                MainClass.contentManager.AuditLogManager.Create(new LicenseDisableEntryData(MainClass.contentManager.AccountLicenseManager.GetLicenseKeyById(keyId).Result), account).Wait();
            }

            return Json(new ObjectResponse<LicenseKeyActionResult>
            {
                Success = true,
                Data = response
            }, MainClass.serializerOptions);
        }

        [HttpGet("enableKey")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<LicenseKeyActionResult>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult EnableKey(string token, string keyId)
        {
            LicenseKeyActionResult response = MainClass.contentManager.AccountLicenseManager.EnableLicenseKey(keyId).Result;

            var account = MainClass.contentManager.AccountManager.GetAccount(token);
            if (response == LicenseKeyActionResult.Success)
            {
                MainClass.contentManager.AuditLogManager.Create(new LicenseEnableEntryData(MainClass.contentManager.AccountLicenseManager.GetLicenseKeyById(keyId).Result), account).Wait();
            }
            return Json(new ObjectResponse<LicenseKeyActionResult>
            {
                Success = true,
                Data = response
            }, MainClass.serializerOptions);
        }
    }
}
