using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using OSLCommon;
using OSLCommon.Authorization;
using System.IO;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text.Json;
using OSLCommon.Logging;

namespace OpenSoftwareLauncher.Server.Controllers.Admin
{
    [Route("admin/serviceaccount")]
    [ApiController]
    [OSLAuthRequired]
    [OSLAuthPermission(AccountPermission.SERVICEACCOUNT_MANAGE)]
    public class ServiceAccountController : Controller
    {
        [HttpPost("create")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<GrantTokenResponse>))]
        [ProducesResponseType(400, Type = typeof(ObjectResponse<HttpException>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult CreateAccount(string token)
        {
            var syncIOFeature = HttpContext.Features.Get<IHttpBodyControlFeature>();
            if (syncIOFeature != null)
            {
                syncIOFeature.AllowSynchronousIO = true;
            }
            ServiceAccountCreateRequest decodedBody;
            try
            {
                StreamReader reader = new StreamReader(Request.Body);
                string decodedBodyText = reader.ReadToEnd();
                decodedBody = JsonSerializer.Deserialize<ServiceAccountCreateRequest>(decodedBodyText, MainClass.serializerOptions);
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

            var tokenAccount = MainClass.contentManager.AccountManager.GetAccount(token);

            var emailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,63})+)$");
            if (!emailRegex.Match(decodedBody.Username).Success)
            {
                Response.StatusCode = 400;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(400, ServerStringResponse.AccountUsernameInvalid)
                }, MainClass.serializerOptions);
            }

            var account = MainClass.contentManager.AccountManager.CreateNewAccount(decodedBody.Username);

            if (account == null)
            {
                Response.StatusCode = 400;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(400, ServerStringResponse.AccountCreateFail)
                }, MainClass.serializerOptions);
            }


            account.Permissions = decodedBody.Permissions;
            account.Licenses = decodedBody.Licenses;
            account.IsServiceAccount = true;


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

            MainClass.contentManager.AccountManager.SetAccount(account);

            MainClass.contentManager.AuditLogManager.Create(new ServiceAccountCreateEntryData(account), tokenAccount).Wait();

            return Json(new ObjectResponse<GrantTokenResponse>()
            {
                Success = true,
                Data = grantTokenResponse
            }, MainClass.serializerOptions);
        }
    }
}
