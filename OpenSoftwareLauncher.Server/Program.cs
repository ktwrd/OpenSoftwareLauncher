using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace OpenSoftwareLauncher.Server
{
    public static class Program
    {
        public static string DataDirectory
        {
            get
            {
                string target = Assembly.GetExecutingAssembly().Location ?? Path.Combine(Directory.GetCurrentDirectory(), "config.ini");
                return Path.GetDirectoryName(target) ?? Directory.GetCurrentDirectory();
            }
        }
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    options.RoutePrefix = "swagger/ui";
                });
                Console.WriteLine(@"[OpenSoftwareLauncher.Server] In development mode, so swagger is enabled. SwaggerUI can be accessed at localhost:5010/swagger/ui");
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }

        public static JsonSerializerOptions serializerOptions = new JsonSerializerOptions()
        {
            IgnoreReadOnlyFields = true,
            IgnoreReadOnlyProperties = true,
            IncludeFields = true,
            WriteIndented = true
        };
    }
}