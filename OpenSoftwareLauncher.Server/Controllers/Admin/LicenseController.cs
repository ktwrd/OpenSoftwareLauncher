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
using System.Reflection;

namespace OpenSoftwareLauncher.Server.Controllers.Admin
{
    [Route("admin/license")]
    [ApiController]
    public class LicenseController : Controller
    {
        public static AccountPermission[] RequiredPermissions = new AccountPermission[]
        {
            AccountPermission.LICENSE_MANAGE,
        };

        [HttpPost("generateProductKey")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<CreateLicenseKeyResponse>))]
        [ProducesResponseType(400, Type = typeof(ObjectResponse<HttpException>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult CreateProductKey(string token)
        {
            var authRes = MainClass.ValidatePermissions(token, RequiredPermissions);
            if (authRes != null)
            {
                Response.StatusCode = authRes?.Data.Code ?? 0;
                return Json(authRes, MainClass.serializerOptions);
            }

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
            var authRes = MainClass.ValidatePermissions(token, RequiredPermissions);
            if (authRes != null)
            {
                Response.StatusCode = authRes?.Data.Code ?? 0;
                return Json(authRes, MainClass.serializerOptions);
            }

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
            var authRes = MainClass.ValidatePermissions(token, RequiredPermissions);
            if (authRes != null)
            {
                Response.StatusCode = authRes?.Data.Code ?? 0;
                return Json(authRes, MainClass.serializerOptions);
            }

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
            var authRes = MainClass.ValidatePermissions(token, RequiredPermissions);
            if (authRes != null)
            {
                Response.StatusCode = authRes?.Data.Code ?? 0;
                return Json(authRes, MainClass.serializerOptions);
            }

            LicenseKeyActionResult response = MainClass.contentManager.AccountLicenseManager.DisableLicenseKey(keyId).Result;

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
            var authRes = MainClass.ValidatePermissions(token, RequiredPermissions);
            if (authRes != null)
            {
                Response.StatusCode = authRes?.Data.Code ?? 0;
                return Json(authRes, MainClass.serializerOptions);
            }

            LicenseKeyActionResult response = MainClass.contentManager.AccountLicenseManager.EnableLicenseKey(keyId).Result;

            return Json(new ObjectResponse<LicenseKeyActionResult>
            {
                Success = true,
                Data = response
            }, MainClass.serializerOptions);
        }
    }
}
