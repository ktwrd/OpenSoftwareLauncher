using OSLCommon;
using OSLCommon.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using OpenSoftwareLauncher.Server.OpenSoftwareLauncher.Server;
using System.Linq;

namespace OpenSoftwareLauncher.Server.Controllers.Admin
{

    [Route("admin/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        public static AccountPermission[] RequiredPermissions = new AccountPermission[]
        {
            AccountPermission.USER_LIST
        };

        [HttpGet]
        [Route("list")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<AccountDetailsResponse[]>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult List(string token, string? username=null, SearchMethod usernameSearchType = SearchMethod.Equals, long firstSeenTimestamp=0, long lastSeenTimestamp=long.MaxValue)
        {
            var authRes = MainClass.ValidatePermissions(token, RequiredPermissions);
            if (authRes != null)
            {
                Response.StatusCode = authRes?.Data.Code ?? 0;
                return Json(authRes, MainClass.serializerOptions);
            }

            var detailList = new List<AccountDetailsResponse>();
            foreach (var account in MainClass.contentManager.AccountManager.GetAllAccounts(false))
            {
                if (account.FirstSeenTimestamp >= firstSeenTimestamp
                    && account.LastSeenTimestamp <= lastSeenTimestamp)
                {
                    if (username == null)
                        detailList.Add(account.GetDetails());
                    else
                    {
                        bool cont = false;
                        switch (usernameSearchType)
                        {
                            case SearchMethod.Equals:
                                cont = username == account.Username;
                                break;
                            case SearchMethod.StartsWith:
                                cont = account.Username.StartsWith(username);
                                break;
                            case SearchMethod.EndsWith:
                                cont = account.Username.EndsWith(username);
                                break;
                            case SearchMethod.Includes:
                                cont = account.Username.Contains(username);
                                break;
                            case SearchMethod.IncludesNot:
                                cont = !account.Username.Contains(username);
                                break;
                        }
                        if (cont)
                            detailList.Add(account.GetDetails());
                    }
                }
            }
            return Json(new ObjectResponse<AccountDetailsResponse[]>()
            {
                Success = true,
                Data = detailList.ToArray()
            }, MainClass.serializerOptions);
        }

        [HttpGet("pardon")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<AccountDetailsResponse>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        [ProducesResponseType(404, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult PardonState(string token, string username)
        {
            var authRes = MainClass.ValidatePermissions(token, AccountPermission.USER_DISABLE_MODIFY);
            if (authRes != null)
            {
                Response.StatusCode = authRes?.Data.Code ?? 0;
                return Json(authRes, MainClass.serializerOptions);
            }

            var targetAccount = MainClass.contentManager.AccountManager.GetAccountByUsername(username);
            if (targetAccount == null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(404, ServerStringResponse.AccountNotFound)
                }, MainClass.serializerOptions);
            }

            targetAccount.Pardon();

            return Json(new ObjectResponse<AccountDetailsResponse>()
            {
                Success = true,
                Data = targetAccount.GetDetails()
            }, MainClass.serializerOptions);
        }

        [HttpGet("disable")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<AccountDetailsResponse>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        [ProducesResponseType(404, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult DisableState(string token, string username, string? reason="No reason")
        {
            var authRes = MainClass.ValidatePermissions(token, AccountPermission.USER_DISABLE_MODIFY);
            if (authRes != null)
            {
                Response.StatusCode = authRes?.Data.Code ?? 0;
                return Json(authRes, MainClass.serializerOptions);
            }
            var bannerAccount = MainClass.contentManager.AccountManager.GetAccount(token);
            var targetAccount = MainClass.contentManager.AccountManager.GetAccountByUsername(username);
            if (targetAccount == null)
            {
                Response.StatusCode = StatusCodes.Status404NotFound;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(404, ServerStringResponse.AccountNotFound)
                }, MainClass.serializerOptions);
            }
            if (!ServerConfig.Security_ImmuneUsers.Contains(targetAccount.Username) && username != bannerAccount.Username)
            {
                if (reason != null)
                    targetAccount.DisableAccount(reason);
                else
                    targetAccount.DisableAccount();
            }


            return Json(new ObjectResponse<AccountDetailsResponse>()
            {
                Success = true,
                Data = targetAccount.GetDetails()
            }, MainClass.serializerOptions);
        }
    }
}
