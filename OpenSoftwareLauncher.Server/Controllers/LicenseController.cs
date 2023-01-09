using Microsoft.AspNetCore.Mvc;
using OSLCommon;
using OSLCommon.Authorization;
using OSLCommon.Licensing;
using OSLCommon.Logging;

namespace OpenSoftwareLauncher.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LicenseController : Controller
    {
        [HttpGet("redeem")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<GrantLicenseKeyResponse>))]
        [ProducesResponseType(400, Type = typeof(ObjectResponse<HttpException>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        [OSLAuthRequired]
        public ActionResult Redeem(string token, string key)
        {
            var account = MainClass.GetService<MongoAccountManager>()?.GetAccount(token);
            var data = MainClass.GetService<MongoAccountLicenseManager>()?.GetLicenseKey(key).Result;
            if (data == null)
            {
                Response.StatusCode = 400;
                return Json(new ObjectResponse<HttpException>
                {
                    Success = false,
                    Data = new HttpException(400, ServerStringResponse.InvalidLicenseKey)
                }, MainClass.serializerOptions);
            }

            var responseCode = MainClass.GetService<MongoAccountLicenseManager>()?.GrantLicenseKey(account?.Username, key).Result;
            var response = new GrantLicenseKeyResponse
            {
                Code = responseCode ?? 0,
                Products = data.ProductsApplied,
                Permissions = data.PermissionsApplied
            };

            if (responseCode == GrantLicenseKeyResponseCode.Granted)
            {
                MainClass.GetService<AuditLogManager>()?.Create(new LicenseRedeemEntryData(data), account).Wait();
            }

            return Json(new ObjectResponse<GrantLicenseKeyResponse>
            {
                Success = true,
                Data = response
            }, MainClass.serializerOptions);
        }

        [HttpGet("info")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<GrantLicenseKeyResponse>))]
        [ProducesResponseType(400, Type = typeof(ObjectResponse<HttpException>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        [OSLAuthRequired]
        public ActionResult Info(string token, string key)
        {
            var data = MainClass.GetService<MongoAccountLicenseManager>()?.GetLicenseKey(key).Result;
            if (data == null)
            {
                Response.StatusCode = 400;
                return Json(new ObjectResponse<HttpException>
                {
                    Success = false,
                    Data = new HttpException(400, ServerStringResponse.InvalidLicenseKey)
                }, MainClass.serializerOptions);
            }

            var responseData = new GrantLicenseKeyResponse
            {
                Code = data.Activated ? GrantLicenseKeyResponseCode.AlreadyRedeemed : GrantLicenseKeyResponseCode.Available,
                Products = data.Products,
                Permissions = data.Permissions
            };

            return Json(new ObjectResponse<GrantLicenseKeyResponse>
            {
                Success = true,
                Data = responseData
            }, MainClass.serializerOptions);
        }
    }
}
