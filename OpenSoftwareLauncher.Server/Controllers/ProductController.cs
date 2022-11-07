using Microsoft.AspNetCore.Mvc;

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
            return Json(MainClass.contentManager.GetAllProductIds(), MainClass.serializerOptions);
        }
    }
}
