using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OSLCommon;
using OSLCommon.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenSoftwareLauncher.Server
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class OSLAuthPermissionAttribute : ActionFilterAttribute
    {
        public List<AccountPermission> AccountPermissions;
        public OSLAuthPermissionAttribute(AccountPermission[] permissions)
        {
            AccountPermissions = new List<AccountPermission>();
            AccountPermissions.AddRange(permissions);
        }
        public OSLAuthPermissionAttribute(AccountPermission permission)
        {
            AccountPermissions = new List<AccountPermission>
            {
                permission
            };
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.Query.ContainsKey("token"))
            {
                var token = context.HttpContext.Request.Query["token"].ToString();
                var account = MainClass.GetService<MongoAccountManager>()?.GetAccount(token, true);
                if (account != null)
                {
                    if (account.Enabled)
                    {
                        bool has = false;
                        if (account.Permissions.Contains(AccountPermission.ADMINISTRATOR))
                        {
                            has = true;
                        }
                        else
                        {
                            foreach (var perm in AccountPermissions)
                            {
                                if (account.HasPermission(perm))
                                {
                                    has = true;
                                    break;
                                }
                            }
                        }

                        if (has)
                        {
                            base.OnActionExecuting(context);
                            return;
                        }
                    }
                    else
                    {
                        context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Result = new JsonResult(new ObjectResponse<HttpException>()
                        {
                            Success = false,
                            Data = new HttpException(StatusCodes.Status401Unauthorized, ServerStringResponse.AccountDisabled + "\n====Reason====\n" + account.DisableReasons.OrderBy(v => v.Timestamp).First()?.Message)
                        }, MainClass.serializerOptions);
                    }
                }
            }

            context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Result = new JsonResult(new ObjectResponse<HttpException>()
            {
                Success = false,
                Data = new HttpException(StatusCodes.Status401Unauthorized, ServerStringResponse.InvalidCredential)
            }, MainClass.serializerOptions);
        }
    }
}
