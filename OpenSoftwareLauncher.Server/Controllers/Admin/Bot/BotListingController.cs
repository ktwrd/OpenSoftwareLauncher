using OSLCommon;
using OSLCommon.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace OpenSoftwareLauncher.Server.Controllers.Admin.Bot
{
    [Route("admin/bot")]
    [ApiController]
    public class BotListingController : Controller
    {
        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("list")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<object>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult ListBots(string token)
        {
#if !DEBUG
            Response.StatusCode = StatusCodes.Status403Forbidden;
            return Json(new ObjectResponse<HttpException>()
            {
                Success = false,
                Data = new HttpException(StatusCodes.Status403Forbidden, @"Not Implemented")
            });
#endif
            if (!MainClass.contentManager.AccountManager.AccountHasPermission(token, AccountPermission.ADMINISTRATOR))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(401, ServerStringResponse.InvalidCredential)
                }, MainClass.serializerOptions);
            }
            return Json(new ObjectResponse<object>()
            {
                Success = true,
                Data = new object()
            }, MainClass.serializerOptions);
        }
    }
}
