using OpenSoftwareLauncher.Server.OpenSoftwareLauncher.Server;
using OSLCommon;
using OSLCommon.Authorization;
using OSLCommon.AutoUpdater;
using kate.shared.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Prometheus;
using static Google.Rpc.Context.AttributeContext.Types;
using OSLCommon.Licensing;

namespace OpenSoftwareLauncher.Server
{
    public static class MainClass
    {
        public static bool DebugMode
        {
            get
            {
#if DEBUG
                return true;
#endif
                // This warning is only disabled because DEBUG is set.
#pragma warning disable CS0162 // Unreachable code detected
                return ServerConfig.GetBoolean("General", "Debug", false);
#pragma warning restore CS0162 // Unreachable code detected
            }
        }

        public static WebApplicationBuilder Builder;
        public static WebApplication App;
        /// <summary>
        /// <para>
        /// Key is the Token
        /// </para>
        /// <para>
        /// Value is the SHA256 of (Username + Password)
        /// </para>
        /// </summary>
        public static Dictionary<string, string> ValidTokens = new Dictionary<string, string>();
        public static List<ITokenGranter> TokenGrantList = new List<ITokenGranter>();

        public static ContentManager contentManager;

        public static string DataDirectory
        {
            get
            {
                string target = Assembly.GetExecutingAssembly().Location ?? Path.Combine(Directory.GetCurrentDirectory(), "config.ini");
                return Path.GetDirectoryName(target) ?? Directory.GetCurrentDirectory();
            }
        }

        public static long StartupTimestamp { get; private set; }
        public static void Main(string[] args)
        {
            StartupTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            ServerConfig.Get();
            ServerConfig.OnWrite += (group, key, value) =>
            {
                switch (group)
                {
                    case "Security":
                        AccountManager.DefaultLicenses = ServerConfig.Security_DefaultSignatures;
                        break;
                    case "Provider":
                        AccountManager.TokenGranters.Clear();
                        AccountManager.TokenGranters.Add(new OSLCommon.AuthProviders.URLProvider(ServerConfig.GetString("Authentication", "Provider")));
                        break;
                }
            };
            AccountManager.DefaultLicenses = ServerConfig.Security_DefaultSignatures;
            AppDomain.CurrentDomain.ProcessExit += BeforeExit;
            serializerOptions.Converters.Add(new kate.shared.DateTimeConverterUsingDateTimeOffsetParse());
            serializerOptions.Converters.Add(new kate.shared.DateTimeConverterUsingDateTimeParse());
            contentManager = new ContentManager();
            LoadTokens();
            Builder = WebApplication.CreateBuilder(args);
            Builder.Services.AddControllers();

            if (Builder.Environment.IsDevelopment())
            {
                Builder.Services.AddSwaggerGen();
            }

            App = Builder.Build();

            if (App.Environment.IsDevelopment())
            {
                App.UseSwagger();
                App.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    options.RoutePrefix = "swagger/ui";
                });
                Console.WriteLine($"[OpenSoftwareLauncher.Server] In development mode, so swagger is enabled. SwaggerUI can be accessed at 0.0.0.0:5010/swagger/ui");
            }
            App.Use((context, next) =>
            {
                context.Request.EnableBuffering();
                var possibleAddress = context.Connection.RemoteIpAddress?.ToString() ?? "";
                if (context.Request.Headers.ContainsKey("X-Forwarded-For"))
                    possibleAddress = context.Request.Headers["X-Forwarded-For"];
                else if (context.Request.Headers.ContainsKey("X-Real-IP"))
                    possibleAddress = context.Request.Headers["X-Real-IP"];
                var query = context.Request.Path.ToString();
                if (!query.StartsWith("/token/grant"))
                    query += context.Request.QueryString.ToString();
               Console.WriteLine($"[OpenSoftwareLauncher.Server] {context.Request.Method} {possibleAddress} \"{query}\" \"{context.Request.Headers.UserAgent}\"");
                return next();
            });

            TokenGrantList.Add(new OSLCommon.AuthProviders.URLProvider(ServerConfig.GetString("Authentication", "Provider")));
            AccountManager.TokenGranters.Add(new OSLCommon.AuthProviders.URLProvider(ServerConfig.GetString("Authentication", "Provider")));

            if (ServerConfig.GetBoolean("Telemetry", "Prometheus"))
            {
                App.UseMetricServer();
                App.UseHttpMetrics();
            }

            App.UseAuthorization();
            App.MapControllers();
            App.Run();
        }
        public static ObjectResponse<HttpException>? Validate(string token)
        {
            var tokenAccount = MainClass.contentManager.AccountManager.GetAccount(token, bumpLastUsed: true);
            if (tokenAccount == null)
            {
                return new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(StatusCodes.Status401Unauthorized, ServerStringResponse.InvalidCredential)
                };
            }
            if (!tokenAccount.Enabled)
            {
                return new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(StatusCodes.Status401Unauthorized, ServerStringResponse.AccountDisabled)
                };
            }
            return null;
        }
        public static ObjectResponse<HttpException>? ValidatePermissions(string token, AccountPermission[] permissions)
        {
            if (!contentManager.AccountManager.AccountHasPermission(token, permissions))
            {
                return new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(StatusCodes.Status401Unauthorized, ServerStringResponse.InvalidCredential)
                };
            }
            var tokenAccount = MainClass.contentManager.AccountManager.GetAccount(token, bumpLastUsed: true);
            if (tokenAccount == null)
            {
                return new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(StatusCodes.Status401Unauthorized, ServerStringResponse.InvalidCredential)
                };
            }
            if (!tokenAccount.Enabled)
            {
                return new ObjectResponse<HttpException>()
                {
                    Success = false,
                    Data = new HttpException(StatusCodes.Status401Unauthorized, ServerStringResponse.AccountDisabled)
                };
            }
            return null;
        }
        public static ObjectResponse<HttpException>? ValidatePermissions(string token, AccountPermission permission)
            => ValidatePermissions(token, new AccountPermission[] { permission });

        public static void BeforeExit(object sender, EventArgs e)
        {
            contentManager.DatabaseSerialize();
            contentManager.SystemAnnouncement.OnUpdate();
            contentManager.AccountManager.ForcePendingWrite();
            ServerConfig.Save();
        }

        public static void LoadTokens()
        {
            if (!File.Exists(@"tokens.json"))
            {
                string superuserToken = GeneralHelper.GenerateToken(32);
                File.WriteAllText(@"tokens.json", $"[\"{superuserToken}\"]");
                Console.WriteLine($"======================================== NOTICE ========================================");
                Console.WriteLine($"The superuser token has been generated. Keep it safe, it has the right to do everything");
                Console.WriteLine($"Superuser Token: \"{superuserToken}\"");
                Console.WriteLine($"====================================== END NOTICE ======================================");
                return;
            }
            else
            {
                var content = File.ReadAllText(@"tokens.json");
                var response = JsonSerializer.Deserialize<List<string>>(content, serializerOptions); ;
                if (response == null)
                {
                    Console.Error.WriteLine($"Failed to parse 'tokens.json' with content of\n{content}");
                    return;
                }
                var dict = new Dictionary<string, string>();
                foreach (var i in response)
                    dict.Add(i, "");
                ValidTokens = dict;
            }
        }

        public static JsonSerializerOptions serializerOptions = new JsonSerializerOptions()
        {
            IgnoreReadOnlyFields = false,
            IgnoreReadOnlyProperties = false,
            IncludeFields = true
        };

        public static string[] GetFileList(string directory, string filename)
        {
            var allFiles = new List<string>();
            foreach (var file in Directory.GetFiles(directory))
            {
                if (file.EndsWith(filename))
                    allFiles.Add(file);
            }
            foreach (var dir in Directory.GetDirectories(directory))
            {
                allFiles = new List<string>(allFiles.Concat(GetFileList(dir, filename)));
            }
            return allFiles.ToArray();
        }
    }
}
