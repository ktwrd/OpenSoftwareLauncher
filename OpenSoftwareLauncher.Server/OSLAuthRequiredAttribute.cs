using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OSLCommon;
using System;
using System.Linq;

namespace OpenSoftwareLauncher.Server
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class OSLAuthRequiredAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.Query.ContainsKey("token"))
            {
                var token = context.HttpContext.Request.Query["token"].ToString();
                var account = MainClass.contentManager.AccountManager.GetAccount(token);
                if (account != null && account.Enabled)
                {
                    base.OnActionExecuting(context);
                    return;
                }
                if (account != null)
                {
                    if (account.Enabled)
                    {
                        base.OnActionExecuting(context);
                        return;
                    }
                    context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Result = new JsonResult(new ObjectResponse<HttpException>()
                    {
                        Success = false,
                        Data = new HttpException(StatusCodes.Status401Unauthorized, ServerStringResponse.AccountDisabled + "\n====Reason====\n" + account.DisableReasons.OrderBy(v => v.Timestamp).First()?.Message)
                    }, MainClass.serializerOptions);
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
