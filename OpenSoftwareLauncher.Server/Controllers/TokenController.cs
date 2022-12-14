using OSLCommon;
using OSLCommon.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using OSLCommon.Logging;
using MongoDB.Driver.Linq;
using System.Linq;

namespace OpenSoftwareLauncher.Server.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class TokenController : Controller
    {
        [HttpGet("grant")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<GrantTokenResponse?>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult Grant(string username, string password)
        {
            var possibleAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                possibleAddress = Request.Headers["X-Forwarded-For"];
            else if (Request.Headers.ContainsKey("X-Real-IP"))
                possibleAddress = Request.Headers["X-Real-IP"];

            var accountUsername = MainClass.GetService<MongoAccountManager>()?.GetAccountByUsername(username);
            if (accountUsername != null && accountUsername.IsServiceAccount)
            {
                if (accountUsername.IsServiceAccount)
                {
                    return Json(new ObjectResponse<GrantTokenResponse>()
                    {
                        Success = false,
                        Data = new GrantTokenResponse(ServerStringResponse.AccountTokenGrantFailed, false)
                    }, MainClass.serializerOptions);
                }
                else if (accountUsername.Enabled == false)
                {
                    Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Json(new ObjectResponse<HttpException>()
                    {
                        Success = false,
                        Data = new HttpException(StatusCodes.Status401Unauthorized, ServerStringResponse.AccountDisabled + "\n====Reason====\n" + accountUsername.DisableReasons.OrderBy(v => v.Timestamp).First()?.Message)
                    }, MainClass.serializerOptions);
                }
            }

            var grantTokenResponse = MainClass.GetService<MongoAccountManager>()?.GrantTokenAndOrAccount(
                WebUtility.UrlDecode(username),
                WebUtility.UrlDecode(password),
                userAgent: Request.Headers.UserAgent,
                host: possibleAddress);
            if (!grantTokenResponse?.Success ?? false)
                Response.StatusCode = StatusCodes.Status401Unauthorized;

            if (grantTokenResponse?.Success ?? false)
            {
                var account = MainClass.GetService<MongoAccountManager>()?.GetAccountByUsername(username);
                MainClass.GetService<AuditLogManager>()?.Create(
                    new TokenCreateEntryData(
                        account,
                        grantTokenResponse.Token),
                    account).Wait();
            }

            return Json(new ObjectResponse<GrantTokenResponse?>()
            {
                Success = grantTokenResponse?.Success ?? false,
                Data = grantTokenResponse
            }, MainClass.serializerOptions);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <response code="401">When token is invalid or the account associated is disabled.</response>
        /// <response code="200">When token is valid and account is not disabled.</response>
        /// <param name="token"></param>
        /// <returns>Wether the token is valid. <see cref="ObjectResponse{Boolean}"/></returns>
        [HttpGet("validate")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ObjectResponse<bool>))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ObjectResponse<bool>))]
        [OSLAuthRequiredAttribute]
        public ActionResult Validate(string token)
        {
            if (token.Length < 32 || token.Length > 32)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new ObjectResponse<bool>()
                {
                    Success = false,
                    Data = false
                }, MainClass.serializerOptions);
            }

            try
            {
                var res = MainClass.GetService<MongoAccountManager>()?.ValidateToken(token) ?? false;
                if (!res)
                {
                    Response.StatusCode = StatusCodes.Status401Unauthorized;
                }
                return Json(new ObjectResponse<bool>()
                {
                    Success = res,
                    Data = res
                }, MainClass.serializerOptions);
            }
            catch (Exception)
            { }

            Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Json(new ObjectResponse<bool>()
            {
                Success = false,
                Data = false
            }, MainClass.serializerOptions);
        }

        [HttpGet("details")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<AccountTokenDetailsResponse>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        [OSLAuthRequiredAttribute]
        public ActionResult Details(string token)
        {
            var account = MainClass.GetService<MongoAccountManager>()?.GetAccount(token, true);

            var details = account?.GetTokenDetails(token);
            if (details == null)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(401, ServerStringResponse.InvalidCredential)
                }, MainClass.serializerOptions);
            }

            return Json(new ObjectResponse<AccountTokenDetailsResponse>()
            {
                Success = true,
                Data = details
            }, MainClass.serializerOptions);
        }

        [HttpGet("remove")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<object>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        [OSLAuthRequiredAttribute]
        public ActionResult Reset(string token, bool? all = false)
        {
            var account = MainClass.GetService<MongoAccountManager>()?.GetAccount(token, true);

            if (all ?? false)
            {
                account?.RemoveTokens();
                return Json(new ObjectResponse<object?>()
                {
                    Success = true,
                    Data = null
                }, MainClass.serializerOptions);
            }
            else
            {
                var tokenData = account?.Tokens.Where(v => v.Token == token).FirstOrDefault();
                if (tokenData != null)
                    MainClass.GetService<AuditLogManager>()?.Create(new TokenDeleteEntryData(account, tokenData), account).Wait();
                account?.RemoveToken(token);
                return Json(new ObjectResponse<object?>()
                {
                    Success = true,
                    Data = null
                }, MainClass.serializerOptions);
            }
        }
    }
}
