using OSLCommon;
using OSLCommon.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OpenSoftwareLauncher.Server.Controllers.Account
{
    [Route("[controller]")]
    [ApiController]
    [OSLAuthRequired]
    public class AccountController : Controller
    {
        [HttpGet("/account")]
        [HttpGet("/account/details")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<AccountDetailsResponse>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult AccountDetails(string token)
        {
            var account = MainClass.GetService<MongoAccountManager>()?.GetAccount(token, true);

            return Json(new ObjectResponse<AccountDetailsResponse?>()
            {
                Success = true,
                Data = account?.GetDetails()
            }, MainClass.serializerOptions);
        }
    }
}
