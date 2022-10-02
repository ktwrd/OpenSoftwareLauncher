using OSLCommon;
using OSLCommon.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace OpenSoftwareLauncher.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : Controller
    {
        [HttpGet("available")]
        [Produces(typeof(string[]))]
        public ActionResult FetchAvailable(string? token="")
        {
            var productIDList = new List<string>();
            var contentManager = MainClass.contentManager;
            if (contentManager != null)
            {
                foreach (var pair in contentManager.Published)
                {
                    var allowAdd = true;
                    if (allowAdd && !productIDList.Contains(pair.Value.Release.appID) && pair.Value.Release.appID.Length > 0)
                        productIDList.Add(pair.Value.Release.appID);
                }
            }

            return Json(productIDList.ToArray(), MainClass.serializerOptions);
        }
    }
}
