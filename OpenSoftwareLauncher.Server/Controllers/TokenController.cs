using OSLCommon;
using OSLCommon.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace OpenSoftwareLauncher.Server.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class TokenController : Controller
    {
        [HttpGet("grant")]
        [Produces(typeof(ObjectResponse<bool>))]
        public ActionResult Grant(string username, string password)
        {
            var possibleAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                possibleAddress = Request.Headers["X-Forwarded-For"];
            else if (Request.Headers.ContainsKey("X-Real-IP"))
                possibleAddress = Request.Headers["X-Real-IP"];

            var grantTokenResponse = MainClass.contentManager.AccountManager.GrantTokenAndOrAccount(
                WebUtility.UrlDecode(username),
                WebUtility.UrlDecode(password),
                userAgent: Request.Headers.UserAgent,
                host: possibleAddress);
            if (!grantTokenResponse.Success)
                Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Json(new ObjectResponse<GrantTokenResponse>()
            {
                Success = grantTokenResponse.Success,
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
                var res = MainClass.contentManager.AccountManager.ValidateToken(token);
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
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<AccountTokenDetailsResponse>))]
        public ActionResult Details(string token)
        {
            if (token == null || token.Length < 32 || token.Length > 32)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(401, ServerStringResponse.InvalidCredential)
                }, MainClass.serializerOptions);
            }

            var account = MainClass.contentManager.AccountManager.GetAccount(token, bumpLastUsed: true);
            if (account == null)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
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
                    Data = new HttpException(401, ServerStringResponse.AccountDisabled)
                }, MainClass.serializerOptions);
            }

            var details = account.GetTokenDetails(token);
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
        public ActionResult Reset(string token, bool? all = false)
        {
            if (token == null || token.Length < 32 || token.Length > 32)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(401, ServerStringResponse.InvalidCredential)
                }, MainClass.serializerOptions);
            }

            var account = MainClass.contentManager.AccountManager.GetAccount(token, bumpLastUsed: true);
            if (account == null)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
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

            if (all ?? false)
            {
                account.RemoveTokens();
                return Json(new ObjectResponse<object>()
                {
                    Success = true,
                    Data = null
                }, MainClass.serializerOptions);
            }
            else
            {
                account.RemoveToken(token);
                return Json(new ObjectResponse<object>()
                {
                    Success = true,
                    Data = null
                }, MainClass.serializerOptions);
            }
        }
    }
}
