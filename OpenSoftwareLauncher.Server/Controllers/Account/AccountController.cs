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
            var account = MainClass.contentManager.AccountManager.GetAccount(token ?? "", true);
            if (account == null)
            {
                Response.StatusCode = 401;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(401, ServerStringResponse.InvalidCredential)
                }, MainClass.serializerOptions);
            }
            if (!account.Enabled)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(StatusCodes.Status401Unauthorized, ServerStringResponse.AccountDisabled)
                }, MainClass.serializerOptions);
            }

            return Json(new ObjectResponse<AccountDetailsResponse>()
            {
                Success = true,
                Data = account.GetDetails()
            }, MainClass.serializerOptions);
        }
    }
}
