using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OSLCommon;
using System.Collections.Generic;

namespace OpenSoftwareLauncher.Server.Controllers.Account
{
    [Route("account/token")]
    [ApiController]
    public class TokenResetController : Controller
    {
        [HttpGet("reset")]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<int>))]
        public ActionResult TokenReset(string token, bool allButSupplied=true, bool all=false)
        {
            var account = MainClass.contentManager.AccountManager.GetAccount(token, true);
            if (account == null)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(StatusCodes.Status401Unauthorized, ServerStringResponse.InvalidCredential)
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
