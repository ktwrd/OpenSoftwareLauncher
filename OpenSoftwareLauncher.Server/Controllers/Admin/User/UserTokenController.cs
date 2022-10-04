using OSLCommon;
using OSLCommon.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using static OSLCommon.Authorization.AccountManager;
using Account = OSLCommon.Authorization.Account;

namespace OpenSoftwareLauncher.Server.Controllers.Admin.User
{
    [Route("admin/user/token")]
    [ApiController]
    public class UserTokenController : Controller
    {
        public AccountPermission[] RequiredPermissions = new AccountPermission[]
        {
            AccountPermission.USER_TOKEN_PURGE
        };

        [HttpGet("purge/all")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<Dictionary<string, int>>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult TokenPurgeAll(string token, bool includeGivenToken=false)
        {
            if (!MainClass.contentManager.AccountManager.AccountHasPermission(token, RequiredPermissions, bumpLastUsed: true))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(401, ServerStringResponse.InvalidCredential)
                }, MainClass.serializerOptions);
            }
            var tokenAccount = MainClass.contentManager.AccountManager.GetAccount(token, bumpLastUsed: true);
            if (tokenAccount == null)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(401, ServerStringResponse.InvalidCredential)
                }, MainClass.serializerOptions);
            }
            if (!tokenAccount.Enabled)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(401, ServerStringResponse.AccountDisabled)
                }, MainClass.serializerOptions);
            }
            var usernameDict = new Dictionary<string, int>();
            foreach (var user in MainClass.contentManager.AccountManager.AccountList)
            {
                string[] exclude = Array.Empty<string>();
                if (includeGivenToken)
                    exclude = new string[] { token };
                var value = user.RemoveTokens(exclude);
                if (!usernameDict.ContainsKey(user.Username))
                    usernameDict.Add(user.Username, 0);
                usernameDict[user.Username] += value;
            }

            return Json(new ObjectResponse<Dictionary<string, int>>()
            {
                Success = true,
                Data = usernameDict
            }, MainClass.serializerOptions);
        }

        [HttpGet("purge")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<Dictionary<string, int>>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult TokenPurge(string token, string? username = null, bool? isUsernameFieldRegexPattern = false)
        {
            if (!MainClass.contentManager.AccountManager.AccountHasPermission(token, RequiredPermissions, bumpLastUsed: true))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(401, ServerStringResponse.InvalidCredential)
                }, MainClass.serializerOptions);
            }
            var tokenAccount = MainClass.contentManager.AccountManager.GetAccount(token, bumpLastUsed: true);
            if (tokenAccount == null)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(401, ServerStringResponse.InvalidCredential)
                }, MainClass.serializerOptions);
            }
            if (!tokenAccount.Enabled)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(401, ServerStringResponse.AccountDisabled)
                }, MainClass.serializerOptions);
            }

            OSLCommon.Authorization.Account[] accountArray;

            if ((isUsernameFieldRegexPattern ?? false) && username != null && username.Length > 0)
            {
                if (username == null)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(new ObjectResponse<string>()
                    {
                        Success = false,
                    }, MainClass.serializerOptions);
                }
                Regex expression;
                try
                {
                    expression = new Regex(username);
                }
                catch (Exception except)
                {
                    Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    return Json(new ObjectResponse<HttpException>()
                    {
                        Success = false,
                        Data = new HttpException((int)HttpStatusCode.InternalServerError, @"Failed to create expression", except)
                    }, MainClass.serializerOptions);
                }

                accountArray = MainClass.contentManager.AccountManager.GetAccountsByRegex(expression, AccountField.Username).ToArray();
            }
            // When username is null, that means we want to purge our own tokens.
            else if (username == null)
            {
                accountArray = new OSLCommon.Authorization.Account[]
                {
                    MainClass.contentManager.AccountManager.GetAccount(token)
                };
            }
            else
            {
                accountArray = new OSLCommon.Authorization.Account[]
                {
                    MainClass.contentManager.AccountManager.GetAccountByUsername(username)
                };
            }

            var usernameDict = new Dictionary<string, int>();
            foreach (var a in accountArray)
            {
                usernameDict.Add(a.Username, a.RemoveTokens());
            }

            return Json(new ObjectResponse<Dictionary<string, int>>()
            {
                Success = true,
                Data = usernameDict
            }, MainClass.serializerOptions);
        }
    }
}
