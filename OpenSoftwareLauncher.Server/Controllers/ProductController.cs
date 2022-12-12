using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using OSLCommon.Authorization;
using OSLCommon.AutoUpdater;
using System.Collections.Generic;
using System.Linq;

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
            return Json(MainClass.Provider.GetService<MongoMiddle>()?.GetAllProductIds(), MainClass.serializerOptions);
        }

        [HttpGet("streams")]
        [Produces(typeof(string[]))]
        public ActionResult FetchStreams(string? token, string? appId)
        {
            var signatureList = new List<string>();
            signatureList = signatureList.Concat(AccountManager.DefaultLicenses).ToList();

            var account = MainClass.Provider.GetService<MongoAccountManager>()?.GetAccount(token ?? "");

            var collection = MainClass.Provider.GetService<MongoMiddle>()?.GetReleaseCollection();

            IFindFluent<ReleaseInfo, ReleaseInfo>? result;
            if (account != null && account.Enabled)
            {
                if (appId == null)
                    result = collection.Find(Builders<ReleaseInfo>.Filter.Empty);
                else
                    result = collection.Find(Builders<ReleaseInfo>
                        .Filter
                        .Eq("appID", appId));

                var wh = result.ToList().Where(v => account.HasLicense(v.remoteLocation)).ToList() ?? new List<ReleaseInfo>();
                signatureList = signatureList.Concat(wh.Select(v => v.remoteLocation)).ToList();
            }

            signatureList = signatureList.Distinct().Where(v => v.Length > 1).ToList();


            return Json(signatureList.ToArray(), MainClass.serializerOptions);
        }
    }
}
