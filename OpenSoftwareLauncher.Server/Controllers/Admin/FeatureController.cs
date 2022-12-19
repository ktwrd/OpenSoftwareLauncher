﻿using Microsoft.AspNetCore.Mvc;
using OSLCommon;
using OSLCommon.Authorization;
using OSLCommon.Features;

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
            if (MainClass.ContentManager.FeatureManager.ContainsName(name))
            {
                Response.StatusCode = 409;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(409, ServerStringResponse.FeatureNameExists)
                }, MainClass.serializerOptions);
            }
            if (MainClass.ContentManager.FeatureManager.ContainsURL(url))
            {
                Response.StatusCode = 409;
                return Json(new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(409, ServerStringResponse.FeatureURLExists)
                }, MainClass.serializerOptions);
            }

            MainClass.ContentManager.FeatureManager.Create(name, url);

            return Json(new ObjectResponse<Feature[]>()
            {
                Success = true,
                Data = MainClass.ContentManager.FeatureManager.GetAll()
            }, MainClass.serializerOptions);
        }

        [HttpGet("deleteName")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<Feature[]>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult DeleteName(string token, string name)
        {
            MainClass.ContentManager.FeatureManager.DeleteByName(name);
            return Json(new ObjectResponse<Feature[]>()
            {
                Success = true,
                Data = MainClass.ContentManager.FeatureManager.GetAll()
            }, MainClass.serializerOptions);
        }
        [HttpGet("deleteUrl")]
        [ProducesResponseType(200, Type = typeof(ObjectResponse<Feature[]>))]
        [ProducesResponseType(401, Type = typeof(ObjectResponse<HttpException>))]
        public ActionResult DeleteURL(string token, string url)
        {
            MainClass.ContentManager.FeatureManager.DeleteByURL(url);
            return Json(new ObjectResponse<Feature[]>()
            {
                Success = true,
                Data = MainClass.ContentManager.FeatureManager.GetAll()
            }, MainClass.serializerOptions);
        }
    }
}
