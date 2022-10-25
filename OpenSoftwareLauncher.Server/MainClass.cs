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

        private static string generateKey()
        {
            int len = 5;
            string[] arr = new string[5];
            for (int i = 0; i < arr.Length; i++)
                arr[i] = GeneralHelper.GenerateToken(len);
            return string.Join('-', arr);
        }

        public static LicenseKeyMetadata? GetLicenseKey(string key)
        {
            foreach (var i in contentManager.LicenseKeys.ToArray())
                if (i.Key == key)
                    return i;
            return null;
        }

        public static bool DisableLicenseKey(string keyId)
        {
            foreach (var item in contentManager.LicenseKeys.ToArray())
            {
                if (item.UID != keyId) continue;
                item.Enable = false;
                return true;
            }
            return false;
        }

        public static CreateLicenseKeyResponse CreateLicenseKeys(string author, string[] products, int count = 1, AccountPermission[]? permissions = null,  string note ="", long activateBy=-1, string groupLabel="")
        {
            var licenseArray = new LicenseKeyMetadata[count];
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            string groupId = GeneralHelper.GenerateToken(18);
            for (int i = 0; i < count; i++)
            {
                licenseArray[i] = new LicenseKeyMetadata
                {
                    UID = GeneralHelper.GenerateToken(16),
                    Enable = true,
                    Activated = false,
                    ActivateByTimestamp = activateBy,
                    ActivatedBy = "",
                    ActivateTimestamp = 0,
                    InternalNote = note,
                    Key = generateKey(),
                    Permissions = permissions ?? Array.Empty<AccountPermission>(),
                    Products = products,
                    CreatedTimestamp = timestamp,
                    CreatedBy = author,
                    GroupId = groupId
                };
            }
            contentManager.LicenseKeys = contentManager.LicenseKeys.Concat(licenseArray).ToList();
            if (contentManager.LicenseKeyGroupNote.ContainsKey(groupId))
                contentManager.LicenseKeyGroupNote.Add(groupId, groupLabel);

            contentManager.LicenseKeyGroupNote[groupId] = groupLabel;

            return new CreateLicenseKeyResponse
            {
                Keys = licenseArray,
                GroupId = groupId
            };
        }

        public static GrantLicenseKeyResponseCode GrantLicenseKey(string username, string licenseKey)
        {
            long currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            foreach (var i in contentManager.LicenseKeys)
            {
                if (i.Key == licenseKey && !i.Activated && i.Enable)
                {
                    if (i.ActivateByTimestamp > 1 && currentTimestamp >= i.ActivateByTimestamp)
                        return GrantLicenseKeyResponseCode.Invalid;
                    var account = contentManager.AccountManager.GetAccountByUsername(username);
                    if (account == null)
                        return GrantLicenseKeyResponseCode.Invalid;
                    int existCount = 0;
                    foreach (var l in i.Products)
                        existCount += (account.GrantLicense(l) ? 0 : 1);
                    if (existCount == i.Products.Length)
                        return GrantLicenseKeyResponseCode.AlreadyRedeemed;
                    i.Activated = true;
                    i.ActivatedBy = username;
                    i.ActivateTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    foreach (var perm in i.Permissions)
                        account.GrantPermission(perm);
                    return GrantLicenseKeyResponseCode.Granted;
                }
            }
            return GrantLicenseKeyResponseCode.Invalid;
        }
        

        public static bool CanUserGroupsAccessStream(string[] blacklist, string[] whitelist, Account account)
        {
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
                return true;
            }
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
