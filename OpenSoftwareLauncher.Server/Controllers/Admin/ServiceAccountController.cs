using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OSLCommon;
using OSLCommon.Authorization;
using System.Linq;
using System.Text.RegularExpressions;

namespace OpenSoftwareLauncher.Server.Controllers.Admin
{
    [Route("admin/serviceaccount")]
    [ApiController]
    public class ServiceAccountController : Controller
    {
        public static AccountPermission[] RequiredPermissions = new AccountPermission[]
        {
            AccountPermission.SERVICEACCOUNT_MANAGE,
        };

        [HttpPost("create")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<GrantTokenResponse>))]
        [ProducesResponseType(400, Type = typeof(ObjectResponse<HttpException>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult CreateAccount(string token, [FromBody] ServiceAccountCreateRequest createRequest)
        {
            var authRes = MainClass.ValidatePermissions(token, RequiredPermissions);
            if (authRes != null)
            {
                Response.StatusCode = authRes?.Data.Code ?? 0;
                return Json(authRes, MainClass.serializerOptions);
            }

            var tokenAccount = MainClass.contentManager.AccountManager.GetAccount(token);

            var emailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            if (!emailRegex.Match(createRequest.Username).Success)
            {
                Response.StatusCode = 400;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(400, ServerStringResponse.AccountUsernameInvalid)
                }, MainClass.serializerOptions);
            }

            var account = MainClass.contentManager.AccountManager.CreateNewAccount(tokenAccount.Username);

            if (account == null)
            {
                Response.StatusCode = 400;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(400, ServerStringResponse.AccountCreateFail)
                }, MainClass.serializerOptions);
            }


            account.Permissions = createRequest.Permissions;
            account.Licenses = createRequest.Licenses;


            var possibleAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                possibleAddress = Request.Headers["X-Forwarded-For"];
            else if (Request.Headers.ContainsKey("X-Real-IP"))
                possibleAddress = Request.Headers["X-Real-IP"];

            var freshtoken = new AccountToken(account);
            freshtoken.UserAgent = Request.Headers.UserAgent;
            freshtoken.Host = possibleAddress;

            account.AddToken(freshtoken);

            var grantTokenResponse = new GrantTokenResponse(ServerStringResponse.AccountTokenGranted, true, freshtoken, account.Groups.ToArray(), account.Licenses.ToArray(), account.Permissions.ToArray());

            return Json(new ObjectResponse<GrantTokenResponse>()
            {
                Success = true,
                Data = grantTokenResponse
            }, MainClass.serializerOptions);
        }
    }
}
