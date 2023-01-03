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
using OSLCommon.Helpers;
using OpenSoftwareLauncher.Server.Targets;

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
#else
                // This warning is only disabled because DEBUG is set.
#pragma warning disable CS0162 // Unreachable code detected
                return ServerConfig.GetBoolean("General", "Debug", false);
#pragma warning restore CS0162 // Unreachable code detected
#endif
            }
        }

        public static WebApplicationBuilder Builder { get; private set; }
        public static WebApplication App { get; private set; }
        /// <summary>
        /// <para>
        /// Key is the Token
        /// </para>
        /// <para>
        /// Value is the SHA256 of (Username + Password)
        /// </para>
        /// </summary>
        public static Dictionary<string, string> ValidTokens = new();
        public static List<ITokenGranter> TokenGrantList = new();

        public static ContentManager? ContentManager { get; private set; }

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
            Option<string> option = new(
                            aliases: new string[] { "--dataDirectory", "-d" },
                            description: "Set the data directory. Default is working directory.");
            Option<string> dataDirOption = option;

            Option<bool>? migrateOption = new(
                aliases: new string[] { "--databaseMigrate" },
                description: "Force a database migration, from JSON to MongoDB.");

            RootCommand rootCommand = new(
                description: "Open Software Launcher Backend Server")
            {
                TreatUnmatchedTokensAsErrors = false
            };

            rootCommand.AddOption(dataDirOption);
            rootCommand.AddOption(migrateOption);
            rootCommand.SetHandler(SetDataDirectory, dataDirOption);
            bool forceMigrate = false;
            rootCommand.SetHandler((opt) =>
            {
                if (opt)
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
        private static string[] Arguments = Array.Empty<string>(); 
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
            ContentManager = new ContentManager();
            RunLaunchTarget();
            App?.Run();
        }
        private static void RunLaunchTarget()
        {
            var targets = new Dictionary<string, Task>()
            {
                {"InitializeServices", new Task(InitializeServices) },
                {"CreateSuperuserAccount", new Task(delegate
                {
                    CreateSuperuserAccount();
                }) },
                {"LoadTokens", new Task(LoadTokens) },
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

            targets = targets.Concat(FindAttributes()).ToDictionary(v => v.Key, k => k.Value);

            var startAll = OSLHelper.GetMilliseconds();
            foreach (var pair in targets)
            {
                var start = OSLHelper.GetMilliseconds();
                pair.Value.Start();
                pair.Value.Wait();
                Log.Debug($"{pair.Key} {OSLHelper.GetMilliseconds() - start}ms");
            }
            Log.Debug($"{OSLHelper.GetMilliseconds() - startAll}ms for {targets.Count} target" + (targets.Count > 1 ? "s" : ""));
        }
        internal class Server : IServer
        {
            public IServiceProvider Provider => MainClass.Provider;
            public event ParameterDelegate<WebApplicationBuilder> AspNetCreate_PreBuild;
            public event ParameterDelegate<WebApplication> AspNetCreate_PreRun;
        }
        private static Dictionary<string, Task> FindAttributes()
        {
            var dict = new Dictionary<string, Task>();
            var server = new Server();
            var targetAssemblyList = new List<Assembly>()
            {
                typeof(MainClass).Assembly
            };
            foreach (var targetAssembly in targetAssemblyList)
            {
                var typeList = OSLHelper.GetTypesWithAttribute<LaunchTargetAttribute>(targetAssembly);
                foreach (var item in typeList)
                {
                    if (!item.IsClass)
                        continue;

                    if (!item.IsAssignableTo(typeof(BaseTarget)))
                    {
                        Log.Error($"{item.FullName} does not extend OpenSoftwareLauncher.Server.BaseTarget");
                        continue;
                    }
                    try
                    {
                        var attr = item.GetCustomAttribute<LaunchTargetAttribute>();
                        var instance = (BaseTarget)Activator.CreateInstance(item);
                        var prop = item.GetProperty("Server");
                        prop?.SetValue(instance, server, null);
                        dict.Add("Attr_" + attr?.Name ?? "<unknown>", new Task(delegate
                        {
                            instance?.Register();
                        }));
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Failed to register {item.AssemblyQualifiedName}");
                        Log.Error(ex.ToString());
                        Environment.Exit(10);
                    }
                }
            }

            return dict;
        }

        public static IServiceProvider? Provider = null;
        public static T? GetService<T>() where T : class
        {
            return Provider?.GetRequiredService<T>() ?? null;
        }
        /// <returns>Generated token for Superuser account</returns>
        public static string? CreateSuperuserAccount()
        {
            Account? account = GetService<MongoAccountManager>()?.GetAccountByUsername(AccountManager.SuperuserUsername);
            account ??= GetService<MongoAccountManager>()?.CreateNewAccount(AccountManager.SuperuserUsername);

            if (!account?.HasPermission(AccountPermission.ADMINISTRATOR) ?? false)
            {
                account?.GrantPermission(AccountPermission.ADMINISTRATOR);
            }

            if (account == null || account?.Tokens.Length < 1)
            {
                var tokenResponse = GetService<MongoAccountManager>()?.CreateToken(account, "internal", "127.0.0.1");
                if (tokenResponse?.Success ?? false)
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

        /// <summary>
        /// Shit to run before we quit.
        /// </summary>
        public static void BeforeExit(object? sender, EventArgs e)
        {
            ContentManager?.DatabaseSerialize();
            GetService<MongoSystemAnnouncement>()?.OnUpdate();
            GetService<MongoAccountManager>()?.ForcePendingWrite();
            Save();
        }
        /// <summary>
        /// Save, Usually called before we quit.
        /// </summary>
        public static void Save()
        {
            // Save legacy config
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
            services.AddSingleton<MongoMiddle>()
                    .AddSingleton<MongoClient>(MongoCreate());
            ConfigureServices_Content(services);
            ConfigureServices_AspNet(services);
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
                .AddApplicationPart(Assembly.GetAssembly(typeof(MainClass)) ?? Assembly.GetExecutingAssembly());
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

        public static JsonSerializerOptions serializerOptions = new()
        {
            IgnoreReadOnlyFields = false,
            IgnoreReadOnlyProperties = false,
            IncludeFields = true
        };
    }
}
