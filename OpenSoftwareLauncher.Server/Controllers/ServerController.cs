using OSLCommon;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Reflection;
using OSLCommon.Features;

namespace OpenSoftwareLauncher.Server.Controllers
{
    [ApiController]
    public class ServerController : Controller
    {
        [HttpGet("/version")]
        [HttpGet("/server/version")]
        public string ServerVersion()
        {
            var asm = Assembly.GetAssembly(typeof(ServerController));
            if (asm == null)
            {
                return @"0.0.0.0";
            }

            Version? version = asm.GetName().Version;
            
            if (version == null)
            {
                return @"0.0.0.0";
            }

            return version.ToString();
        }

        [HttpGet("/uptime")]
        [HttpGet("/server/uptime")]
        public long ServerUptime()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds() - MainClass.StartupTimestamp;
        }

        [HttpGet("/")]
        [HttpGet("/server/details")]
        [Produces(typeof(ServerDetailsResponse))]
        public ActionResult ServerDetails()
        {
            return Json(new ServerDetailsResponse
            {
                Uptime = ServerUptime(),
                Version = ServerVersion(),
                AuthProvider = ServerConfig.GetString("Authentication", "Provider"),
                AuthProviderSignup = ServerConfig.GetString("Authentication", "ProviderSignupURL"),
                Features = MainClass.GetService<FeatureManager>()?.GetAll() ?? Array.Empty<Feature>()
            }, MainClass.serializerOptions);
        }
    }
}
