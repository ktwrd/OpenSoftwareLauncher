using OSLCommon;
using OSLCommon.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using static OSLCommon.Authorization.AccountManager;
using OSLCommon.Logging;

namespace OpenSoftwareLauncher.Server.Controllers.Admin.User
{
    [Route("admin/user/token")]
    [ApiController]
    [OSLAuthRequired]
    [OSLAuthPermission(AccountPermission.USER_TOKEN_PURGE)]
    public class UserTokenController : Controller
    {
        [HttpGet("purge/all")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<Dictionary<string, int>>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult TokenPurgeAll(string token, bool includeGivenToken=false)
        {
            var tokenAccount = MainClass.ContentManager.AccountManager.GetAccount(token);
            var usernameDict = new Dictionary<string, int>();
            foreach (var user in MainClass.ContentManager.AccountManager.GetAllAccounts())
            {
                string[] exclude = Array.Empty<string>();
                if (includeGivenToken)
                    exclude = new string[] { token };
                var value = user.RemoveTokens(exclude);
                if (!usernameDict.ContainsKey(user.Username))
                    usernameDict.Add(user.Username, 0);
                usernameDict[user.Username] += value;
            }

            MainClass.ContentManager.AuditLogManager.Create(new BulkTokenDeleteEntryData()
            {
                Dict = usernameDict
            }, tokenAccount).Wait();

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
            var tokenAccount = MainClass.ContentManager.AccountManager.GetAccount(token);

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

                accountArray = MainClass.ContentManager.AccountManager.GetAccountsByRegex(expression, AccountField.Username);
            }
            // When username is null, that means we want to purge our own tokens.
            else if (username == null)
            {
                accountArray = new OSLCommon.Authorization.Account[]
                {
                    MainClass.ContentManager.AccountManager.GetAccount(token)
                };
            }
            else
            {
                accountArray = new OSLCommon.Authorization.Account[]
                {
                    MainClass.ContentManager.AccountManager.GetAccountByUsername(username)
                };
            }

            var usernameDict = new Dictionary<string, int>();
            foreach (var a in accountArray)
            {
                usernameDict.Add(a.Username, a.RemoveTokens());
            }
            MainClass.ContentManager.AuditLogManager.Create(new BulkTokenDeleteEntryData()
            {
                Dict = usernameDict
            }, tokenAccount).Wait();

            return Json(new ObjectResponse<Dictionary<string, int>>()
            {
                Success = true,
                Data = usernameDict
            }, MainClass.serializerOptions);
        }
    }
}
