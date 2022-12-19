using Microsoft.AspNetCore.Mvc;
using OSLCommon;
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
            var account = MainClass.ContentManager.AccountManager.GetAccount(token);
            var data = MainClass.ContentManager.AccountLicenseManager.GetLicenseKey(key).Result;
            if (data == null)
            {
                Response.StatusCode = 400;
                return Json(new ObjectResponse<HttpException>
                {
                    Success = false,
                    Data = new HttpException(400, ServerStringResponse.InvalidLicenseKey)
                }, MainClass.serializerOptions);
            }

            var responseCode = MainClass.ContentManager.AccountLicenseManager.GrantLicenseKey(account.Username, key).Result;
            var response = new GrantLicenseKeyResponse
            {
                Code = responseCode,
                Products = data.ProductsApplied,
                Permissions = data.PermissionsApplied
            };

            if (responseCode == GrantLicenseKeyResponseCode.Granted)
            {
                MainClass.ContentManager.AuditLogManager.Create(new LicenseRedeemEntryData(data), account).Wait();
            }

            return Json(new ObjectResponse<GrantLicenseKeyResponse>
            {
                Success = true,
                Data = response
            }, MainClass.serializerOptions);
        }
    }
}
