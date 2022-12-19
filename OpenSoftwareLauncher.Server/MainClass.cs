using OSLCommon;
using OSLCommon.Authorization;
using kate.shared.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Prometheus;
using OSLCommon.AuthProviders;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Runtime.CompilerServices;
using MongoDB.Driver;
using OSLCommon.Logging;
using OSLCommon.Licensing;
using OSLCommon.Features;
using System.Threading.Tasks;

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

        private static string? dataDirectory = null;
        public static string DataDirectory
        {
            get
            {
                string target = dataDirectory ?? Directory.GetCurrentDirectory();
                return target;
            }
        }

        public static long StartupTimestamp { get; private set; }
        private static void SetupOptions(params string[] args)
        {

            Option<string> dataDirOption = new Option<string>(
                aliases: new string[] { "--dataDirectory", "-d" },
                description: "Set the data directory. Default is working directory.");

            Option<bool>? migrateOption = new Option<bool>(
                aliases: new string[] { "--databaseMigrate" },
                description: "Force a database migration, from JSON to MongoDB.");

            RootCommand rootCommand = new RootCommand(
                description: "Open Software Launcher Backend Server");
            rootCommand.TreatUnmatchedTokensAsErrors = false;

            rootCommand.AddOption(dataDirOption);
            rootCommand.AddOption(migrateOption);
            rootCommand.SetHandler(SetDataDirectory, dataDirOption);
            bool forceMigrate = false;
            rootCommand.SetHandler((opt) =>
            {
                if (opt != null && opt)
                {
                    forceMigrate = true;
                }
            }, migrateOption);
            rootCommand.Invoke(args);

            if (forceMigrate)
            {
                foreach (var parent in ServerConfig.DefaultData)
                {
                    if (parent.Key != "Migrated")
                        continue;
                    foreach (var child in parent.Value)
                    {
                        ServerConfig.Set(parent.Key, child.Key, child.Value);
                    }
                }
                Log.WriteLine($" Enforced Mirgration");
            }
        }
        public static void SetDataDirectory(string dataDirectory)
        {
            if (dataDirectory == null) return;
            MainClass.dataDirectory = dataDirectory.Trim('"');
            Log.WriteLine($" Set data directory to \"{MainClass.dataDirectory}\"");
        }
        private static void PrintConfig()
        {
            foreach (var parent in ServerConfig.Get())
            {
                foreach (var child in parent.Value)
                {
                    Log.WriteLine($" {parent.Key}.{child.Key} = {child.Value}");
                }
            }
        }
        private static string[] Arguments; 
        public static void Main(params string[] args)
        {
            Arguments = args;
            SetupOptions(args);
            StartupTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
#if DEBUG
            PrintConfig();
#endif
            ServerConfig.Get();
            if (ServerConfig.GetString("Connection", "MongoDBServer", "").Length < 1)
            {
                Log.WriteLine($" MongoDB Connection URL is invalid. Please set it in `config.ini`");
                Environment.Exit(1);
            }
            ServerConfig.OnWrite += (group, key, value) =>
            {
                switch (group)
                {
                    case "Security":
                        AccountManager.DefaultLicenses = ServerConfig.Security_DefaultSignatures;
                        break;
                    case "Provider":
                        AccountManager.TokenGranters.Clear();
                        TokenGrantList.Clear();
                        TokenGrantList.Add(new URLProvider(ServerConfig.GetString("Authentication", "Provider")));
                        AccountManager.TokenGranters.Add(new URLProvider(ServerConfig.GetString("Authentication", "Provider")));
                        break;
                }
            };
            AccountManager.DefaultLicenses = ServerConfig.Security_DefaultSignatures;
            AppDomain.CurrentDomain.ProcessExit += BeforeExit;
            serializerOptions.Converters.Add(new kate.shared.DateTimeConverterUsingDateTimeOffsetParse());
            serializerOptions.Converters.Add(new kate.shared.DateTimeConverterUsingDateTimeParse());
            contentManager = new ContentManager();
            var targets = new Dictionary<string, Task>()
            {
                {"InitializeASPNetEvents", new Task(InitASPNETEvents) },
                {"CreateSuperuserAccount", new Task(delegate
                {
                    CreateSuperuserAccount();
                }) },
                {"LoadTokens", new Task(LoadTokens) },
                {"InitializeServices", new Task(InitializeServices) },
                {"LegacyImport", new Task(delegate
                {
                    LegacyImport.Execute().Wait();
                }) },
                {"TokenGranter", new Task(delegate
                {
                    TokenGrantList.Add(new URLProvider(ServerConfig.GetString("Authentication", "Provider")));
                    AccountManager.TokenGranters.Add(new URLProvider(ServerConfig.GetString("Authentication", "Provider")));
                }) }
            };

            foreach (var pair in targets)
            {
                var start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                pair.Value.Start();
                pair.Value.Wait();
                Log.Debug($"{pair.Key} {DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - start}ms");
            }

            App.Run();
        }
        private static void InitASPNETEvents()
        {
            AspNetCreate_PreBuild += AspNetTarget_Swagger_PreBuild;
            AspNetCreate_PreRun   += AspNetTarget_RequestLog;
            AspNetCreate_PreRun   += AspNetTarget_Swagger;
            AspNetCreate_PreRun   += AspNetTarget_Prometheus;
        }
        public static IServiceProvider Provider;
        /// <returns>Token</returns>
        public static string? CreateSuperuserAccount()
        {
            Account account = contentManager.AccountManager.GetAccountByUsername(AccountManager.SuperuserUsername);
            if (account == null)
            {
                account = contentManager.AccountManager.CreateNewAccount(AccountManager.SuperuserUsername);
            }

            if (!account.HasPermission(AccountPermission.ADMINISTRATOR))
            {
                account.GrantPermission(AccountPermission.ADMINISTRATOR);
            }

            if (account.Tokens.Length < 1)
            {
                var tokenResponse = contentManager.AccountManager.CreateToken(account, "internal", "127.0.0.1");
                if (tokenResponse.Success)
                {
                    Console.WriteLine($"================================================================================");
                    Console.WriteLine($"Created superuser account.");
                    Console.WriteLine($"Username: {AccountManager.SuperuserUsername}");
                    Console.WriteLine($"Token:    {tokenResponse.Token.Token}");
                    Console.WriteLine($"================================================================================");
                    File.WriteAllText(Path.Join(DataDirectory, "superuser-token.txt"), tokenResponse.Token.Token);
                    return tokenResponse.Token.Token;
                }
            }
            return null;
        }

        public static void BeforeExit(object? sender, EventArgs e)
        {
            contentManager.DatabaseSerialize();
            contentManager.SystemAnnouncement.OnUpdate();
            contentManager.AccountManager.ForcePendingWrite();
            ServerConfig.Save();
        }

        private static void InitializeServices()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            Provider = services.BuildServiceProvider();

            var sa = Provider.GetService<MongoSystemAnnouncement>();
            if (sa != null)
            {
                sa.Update += delegate
                {
                    ServerConfig.Set("Announcement", "Enable", sa.Active);
                };
            }
        }
        private static void ConfigureServices(IServiceCollection services)
        {
            ConfigureServices_AspNet(services);
            services.AddSingleton<MongoMiddle>()
                    .AddSingleton<MongoClient>(MongoCreate())
                    .AddSingleton(App);
            ConfigureServices_Content(services);
        }

        private static MongoClient MongoCreate()
        {
            Log.Debug($"Connecting to Database");
            MongoClient client = new(ServerConfig.GetString("Connection", "MongoDBServer"));
            client.StartSession();
            return client;
        }
        private static void ConfigureServices_Content(IServiceCollection services)
        {
            services.AddSingleton<AuditLogManager>()
                    .AddSingleton<MongoAccountManager>()
                    .AddSingleton<MongoSystemAnnouncement>()
                    .AddSingleton<MongoAccountLicenseManager>()
                    .AddSingleton<FeatureManager>();
        }
        private static void ConfigureServices_AspNet(IServiceCollection services)
        {
            Builder = WebApplication.CreateBuilder(Arguments);
            Builder.Services.AddControllers()
                .AddApplicationPart(Assembly.GetAssembly(typeof(MainClass)));
            if (Builder.Environment.IsDevelopment())
                Builder.Services.AddSwaggerGen();
            AspNetCreate_PreBuild?.Invoke(Builder);

            App = Builder.Build();
            AspNetCreate_PreRun?.Invoke(App);
            App.UseAuthorization();
            App.MapControllers();

            services.AddSingleton(App);
        }
        public static ParameterDelegate<WebApplicationBuilder>? AspNetCreate_PreBuild;
        public static ParameterDelegate<WebApplication>? AspNetCreate_PreRun;

        #region ASP.NET Targets
        private static void AspNetTarget_Swagger_PreBuild(WebApplicationBuilder builder)
        {
            if (Builder.Environment.IsDevelopment())
            {
                Builder.Services.AddSwaggerGen();
            }
        }
        private static void AspNetTarget_RequestLog(WebApplication app)
        {
            app.Use((context, next) =>
            {
                context.Request.EnableBuffering();
                string possibleAddress = ServerHelper.FindClientAddress(context);
                string userAgent = context.Request.Headers.UserAgent;

                var query = context.Request.Path.ToString();
                if (!query.Contains("&password"))
                    query += context.Request.QueryString.ToString();
                
                Log.WriteLine($" {context.Request.Method} {possibleAddress} \"{query}\" \"{userAgent}\"");
                return next();
            });
        }
        private static void AspNetTarget_Swagger(WebApplication app)
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
        private static void AspNetTarget_Prometheus(WebApplication app)
        {
            if (ServerConfig.GetBoolean("Telemetry", "Prometheus"))
            {
                App.UseMetricServer();
                App.UseHttpMetrics();
            }
            else
            {
                Log.Warn("Prometheus Exporter is disabled");
            }
        }
        #endregion


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
                    Log.Error($"Failed to parse 'tokens.json' with content of\n{content}");
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
