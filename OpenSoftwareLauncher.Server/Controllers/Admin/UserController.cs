using OSLCommon;
using OSLCommon.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using OSLCommon.Logging;

namespace OpenSoftwareLauncher.Server.Controllers.Admin
{

    [Route("admin/[controller]")]
    [ApiController]
    [OSLAuthRequired]
    [OSLAuthPermission(AccountPermission.USER_LIST)]
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
        [OSLAuthRequired]
        [OSLAuthPermission(AccountPermission.USER_LIST)]
        public ActionResult List(string token, string? username=null, SearchMethod usernameSearchType = SearchMethod.Equals, long firstSeenTimestamp=0, long lastSeenTimestamp=long.MaxValue)
        {
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

        [HttpGet("delete")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<object>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        [OSLAuthRequired]
        [OSLAuthPermission(AccountPermission.USER_DELETE)]
        public ActionResult DeleteAccount(string token, string username)
        {
            MainClass.contentManager.AccountManager.DeleteAccount(username);
            return Json(new ObjectResponse<object>()
            {
                Success = true,
                Data = new object()
            }, MainClass.serializerOptions);
        }

        [HttpGet("pardon")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<AccountDetailsResponse>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        [ProducesResponseType(404, Type = typeof(ObjectResponse<HttpException>))]
        [OSLAuthRequired]
        [OSLAuthPermission(AccountPermission.USER_DISABLE_MODIFY)]
        public ActionResult PardonState(string token, string username)
        {
            var tokenAccount = MainClass.contentManager.AccountManager.GetAccount(token);
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

            MainClass.contentManager.AuditLogManager.Create(new AccountDisableEntryData(targetAccount)
            {
                Reason = "",
                State = false
            }, tokenAccount).Wait();

            MainClass.contentManager.AuditLogManager.Create(new AccountDeleteEntryData()
            {
                Username = username
            }, tokenAccount).Wait();

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
        [OSLAuthRequired]
        [OSLAuthPermission(AccountPermission.USER_DISABLE_MODIFY)]
        public ActionResult DisableState(string token, string username, string? reason="No reason")
        {
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

            MainClass.contentManager.AuditLogManager.Create(new AccountDisableEntryData(targetAccount)
            {
                Reason = reason,
                State = true
            }, bannerAccount).Wait();


            return Json(new ObjectResponse<AccountDetailsResponse>()
            {
                Success = true,
                Data = targetAccount.GetDetails()
            }, MainClass.serializerOptions);
        }
    }
}
