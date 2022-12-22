using Microsoft.AspNetCore.Mvc;
using OSLCommon;
using OSLCommon.Authorization;
using OSLCommon.Features;
using System;

namespace OpenSoftwareLauncher.Server.Controllers.Admin
{
    [ApiController]
    [Route("admin/feature")]
    [OSLAuthRequired]
    [OSLAuthPermission(AccountPermission.ADMINISTRATOR)]
    public class FeatureController : Controller
    {
        [HttpGet("add")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<Feature[]>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        [ProducesResponseType(409, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult Add(string token, string name, string url)
        {
            if (MainClass.GetService<FeatureManager>()?.ContainsName(name) ?? false)
            {
                Response.StatusCode = 409;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(409, ServerStringResponse.FeatureNameExists)
                }, MainClass.serializerOptions);
            }
            if (MainClass.GetService<FeatureManager>()?.ContainsURL(url) ?? false)
            {
                Response.StatusCode = 409;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(409, ServerStringResponse.FeatureURLExists)
                }, MainClass.serializerOptions);
            }

            MainClass.GetService<FeatureManager>()?.Create(name, url);

            return Json(new ObjectResponse<Feature[]>()
            {
                Success = true,
                Data = MainClass.GetService<FeatureManager>()?.GetAll() ?? Array.Empty<Feature>()
            }, MainClass.serializerOptions);
        }

        [HttpGet("deleteName")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<Feature[]>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult DeleteName(string token, string name)
        {
            MainClass.GetService<FeatureManager>()?.DeleteByName(name);
            return Json(new ObjectResponse<Feature[]>()
            {
                Success = true,
                Data = MainClass.GetService<FeatureManager>()?.GetAll() ?? Array.Empty<Feature>()
            }, MainClass.serializerOptions);
        }
        [HttpGet("deleteUrl")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<Feature[]>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult DeleteURL(string token, string url)
        {
            MainClass.GetService<FeatureManager>()?.DeleteByURL(url);
            return Json(new ObjectResponse<Feature[]>()
            {
                Success = true,
                Data = MainClass.GetService<FeatureManager>()?.GetAll() ?? Array.Empty<Feature>()
            }, MainClass.serializerOptions);
        }
    }
}
