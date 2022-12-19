using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prometheus;

namespace OpenSoftwareLauncher.Server.Targets
{
    public class ASPNetTarget
    {
        public ASPNetTarget()
        {
            MainClass.AspNetCreate_PreBuild += Swagger_PreBuild;
            MainClass.AspNetCreate_PreRun += RequestLog;
            MainClass.AspNetCreate_PreRun += Swagger;
            MainClass.AspNetCreate_PreRun += Prometheus;
        }
        private static void Swagger_PreBuild(WebApplicationBuilder builder)
        {
            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddSwaggerGen();
            }
        }
        private static void RequestLog(WebApplication app)
        {
            app.Use((context, next) =>
            {
                string possibleAddress = ServerHelper.FindClientAddress(context);
                string userAgent = context.Request.Headers.UserAgent;

                var query = context.Request.Path.ToString();
                if (!query.Contains("&password"))
                    query += context.Request.QueryString.ToString();

                Log.WriteLine($" {context.Request.Method} {possibleAddress} \"{query}\" \"{userAgent}\"");
                return next();
            });
        }
        private static void Swagger(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    options.RoutePrefix = "swagger/ui";
                });
                Log.WriteLine($" In development mode, so swagger is enabled. SwaggerUI can be accessed at 0.0.0.0:5010/swagger/ui");
            }
            else
            {
                Log.Error("In production mode, not enabling swagger");
            }
        }
        private static void Prometheus(WebApplication app)
        {
            if (ServerConfig.GetBoolean("Telemetry", "Prometheus"))
            {
                app.UseMetricServer();
                app.UseHttpMetrics();
            }
            else
            {
                Log.Warn("Prometheus Exporter is disabled");
            }
        }
    }
}
