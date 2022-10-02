using OSLCommon;
using OSLCommon.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;

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
            if (!MainClass.contentManager.AccountManager.AccountHasPermission(token, RequiredPermissions))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new ObjectResponse<string>()
                {
                    Success = false,
                    Data = "Invalid Account"
                }, MainClass.serializerOptions);
            }

            var detailList = new List<AccountDetailsResponse>();
            foreach (var account in MainClass.contentManager.AccountManager.AccountList)
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

    }
}
