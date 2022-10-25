using OSLCommon;
using OSLCommon.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace OpenSoftwareLauncher.Server.Controllers.Account
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        [HttpGet("/account")]
        [HttpGet("/account/details")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<AccountDetailsResponse>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult AccountDetails(string token)
        {
            var authRes = MainClass.Validate(token);
            if (authRes != null)
            {
                Response.StatusCode = authRes?.Data.Code ?? 0;
                return Json(authRes, MainClass.serializerOptions);
            }
            var account = MainClass.contentManager.AccountManager.GetAccount(token, true);

            return Json(new ObjectResponse<AccountDetailsResponse>()
            {
                Success = true,
                Data = account.GetDetails()
            }, MainClass.serializerOptions);
        }
    }
}
