using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OSLCommon;
using OSLCommon.Licensing;

namespace OpenSoftwareLauncher.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LicenseController : Controller
    {
        [HttpGet("redeem")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<GrantLicenseKeyResponse>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult Redeem(string token, string key)
        {
            var authRes = MainClass.Validate(token);
            if (authRes != null)
            {
                Response.StatusCode = authRes?.Data.Code ?? 0;
                return Json(authRes, MainClass.serializerOptions);
            }
            var account = MainClass.contentManager.AccountManager.GetAccount(token);
            var data = MainClass.GetLicenseKey(key);
            if (data == null)
            {
                Response.StatusCode = 400;
                return Json(new ObjectResponse<HttpException>
                {
                    Success = false,
                    Data = new HttpException(400, ServerStringResponse.InvalidLicenseKey)
                });
            }

            var responseCode = MainClass.GrantLicenseKey(account.Username, key);
            var response = new GrantLicenseKeyResponse
            {
                Code = responseCode,
                Products = data.Products
            };

            return Json(new ObjectResponse<GrantLicenseKeyResponse>
            {
                Success = true,
                Data = response
            }, MainClass.serializerOptions);
        }
    }
}
