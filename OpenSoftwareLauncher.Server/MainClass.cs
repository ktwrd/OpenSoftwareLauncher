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

        public static void BeforeExit(object sender, EventArgs e)
        {
            contentManager.DatabaseSerialize();
            contentManager.SystemAnnouncement.OnUpdate();
            contentManager.AccountManager.ForcePendingWrite();
            ServerConfig.Save();
        }

        public static bool CanUserGroupsAccessStream(string[] blacklist, string[] whitelist, Account account)
        {
            bool allow = false;
            
            if (ServerConfig.GetBoolean("Security", "EnableGroupRestriction", false))
            {
                bool userHasWhitelist = false;
                bool userHasBlacklist = false;
                foreach (var group in blacklist)
                {
                    if (account.Groups.Contains(group))
                        userHasBlacklist = true;
                }
                foreach (var group in whitelist)
                {
                    if (account.Groups.Contains(group))
                        userHasWhitelist = true;
                }

                if (blacklist.Length > 0 && userHasBlacklist)
                    return false;
                if (whitelist.Length > 0 && userHasWhitelist)
                    return true;
                if (whitelist.Length > 0 && userHasWhitelist == false)
                    return false;
                return true;
            }
            else
            {
                allow = true;
            }

            return allow;
        }

        internal static List<AuthenticatedUser> Accounts = new List<AuthenticatedUser>();
        public static AuthenticatedUser? FetchUser(string username, string password)
        {
            SHA256 sha256Instance = SHA256.Create();
            var computedHash = sha256Instance.ComputeHash(Encoding.UTF8.GetBytes($"{username}{password}"));
            var hash = GeneralHelper.Base62Encode(computedHash);
            foreach (var account in Accounts)
            {
                if (account.ValidateHash(hash))
                    return account;
            }
            return null;
        }
        public static AuthenticatedUser? FetchUserByToken(string token)
        {
            foreach (var account in Accounts)
            {
                if (account.Token == token)
                    return account;
            }
            return null;
        }
        public static bool UserHasService(AuthenticatedUser user, string service) => user.AvailableServices.Contains(service);

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
        public static bool UserByTokenHasService(string token, string service)
        {
            if (token.Length < 1)
                return false;
            var targetAccount = FetchUserByToken(token);
            if (targetAccount == null)
                return false;
            if (targetAccount.IsAdmin)
                return true;
            return targetAccount.AvailableServices.Contains(service);
        }
        public static bool UserByTokenIsAdmin(string token)
        {
            var targetAccount = FetchUserByToken(token);
            if (targetAccount == null)
                return false;
            return targetAccount.IsAdmin;
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

        public static List<ReleaseInfo> ScrapeForProducts(string[] infoFiles)
        {
            var releaseList = new List<ReleaseInfo>();
            foreach (var file in infoFiles)
            {
                var content = File.ReadAllText(file);
                if (content.Replace(": ", ":").Contains("\"envtimestamp\":\"")) continue;
                var deserialized = JsonSerializer.Deserialize<ReleaseInfo>(content, serializerOptions);
                if (deserialized == null || deserialized?.envtimestamp == null) continue;

                var splitted = file.Split(Path.DirectorySeparatorChar);
                var organization = splitted[splitted.Length - 4];
                var repository = splitted[splitted.Length - 3];
                if (deserialized.remoteLocation.Length < 1)
                    deserialized.remoteLocation = $"{organization}/{repository}";

                var recognitionMap = new Dictionary<ReleaseType, string[]>()
                {
                    {ReleaseType.Beta, new string[] {
                        "-beta",
                        "-canary"
                    } },
                    {ReleaseType.Nightly, new string[] {
                        "-dev",
                        "-devel",
                        "-debug",
                        "-nightly"
                    } },
                    {ReleaseType.Stable, new string[] {
                        "-stable",
                        "-public",
                        "-prod",
                        "-main"
                    } }
                };
                var targetReleaseType = ReleaseType.Invalid;

                foreach (var pair in recognitionMap)
                {
                    var pairTarget = ReleaseType.Invalid;
                    foreach (var item in pair.Value)
                    {
                        if (pairTarget != ReleaseType.Invalid && (repository.EndsWith(item) || deserialized.remoteLocation.EndsWith(item)))
                            pairTarget = pair.Key;
                    }
                    if (pairTarget != ReleaseType.Invalid)
                    {
                        targetReleaseType = pairTarget;
                        break;
                    }
                }
                if (repository.Split('-').Length == 1 || deserialized.remoteLocation.Split('-').Length == 1)
                    targetReleaseType = ReleaseType.Stable;
                else if (targetReleaseType == ReleaseType.Invalid)
                    targetReleaseType = ReleaseType.Other;

                deserialized.releaseType = targetReleaseType;
                if ((deserialized.files.ContainsKey("windows") && deserialized.executable.ContainsKey("windows")) ||
                    (deserialized.files.ContainsKey("linux")   && deserialized.executable.ContainsKey("linux")))
                    releaseList.Add(deserialized);
            }
            return releaseList;
        }

        public static List<ProductRelease> Products = new List<ProductRelease>();
    }
}
