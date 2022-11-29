using Microsoft.AspNetCore.Mvc;
using OSLCommon;

namespace OpenSoftwareLauncher.Server.Controllers.Account
{
    [Route("account/token")]
    [ApiController]
    [OSLAuthRequired]
    public class TokenResetController : Controller
    {
        [HttpGet("reset")]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<int>))]
        public ActionResult TokenReset(string token, bool allButSupplied=true, bool all=false)
        {
            var account = MainClass.contentManager.AccountManager.GetAccount(token, true);

            int tokensRemoved = 0;

            if (all)
            {
                tokensRemoved = account.RemoveTokens();
            }
            else if (allButSupplied)
            {
                tokensRemoved = account.RemoveTokens(new string[] { token });
            }

            return Json(new ObjectResponse<int>()
            {
                Success = true,
                Data = tokensRemoved
            }, MainClass.serializerOptions);
        }
    }
}
