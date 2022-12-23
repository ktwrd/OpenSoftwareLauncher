using kate.shared.Helpers;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using OSLCommon.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace OSLCommon.Authorization
{
    public class MongoAccountManager : AccountManager
    {
        public MongoClient mongoClient;
        public AuditLogManager auditLogManager;
        public string DatabaseName = "opensoftwarelauncher";
        public string CollectionName = "accounts";
        public string CollectionName_PermissionGroup = "permissionGroups";
        public MongoAccountManager(IServiceProvider provider)
            : base()
        {
            mongoClient = provider.GetService<MongoClient>();
            auditLogManager = provider.GetService<AuditLogManager>();
            AccountUpdated += (eventAccount) =>
            {
                var collection = GetAccountCollection<Account>();

                var filter = Builders<Account>.Filter.Where(v => v.Username == eventAccount.Username);

                collection.ReplaceOne(filter, eventAccount);
            };
        }
        private IMongoCollection<T> GetAccountCollection<T>()
        {
            return mongoClient.GetDatabase(DatabaseName).GetCollection<T>(CollectionName);
        }
        private IMongoCollection<T> GetPermissionGroupCollection<T>()
        {
            return mongoClient.GetDatabase(DatabaseName).GetCollection<T>(CollectionName_PermissionGroup);
        }
        internal void HookAccountEvent(Account account)
        {
            if (account == null || account.eventHook) return;
            account.eventHook = true;
            AccountUpdated += (eventAccount) =>
            {
                if (eventAccount.Username == account.Username)
                {
                    account.Merge(eventAccount);
                }
            };
        }

        public JsonSerializerOptions serializerOptions = new JsonSerializerOptions
        {
            IgnoreReadOnlyFields = true,
            IgnoreReadOnlyProperties = true,
            IncludeFields = true
        };

        public override GrantTokenResponse CreateToken(Account account, string userAgent = "", string host = "", string tokenGranter = "<none>")
        {
            var b = base.CreateToken(account, userAgent, host);
            return b;
        }

        #region Account Boilerplate

        internal override Account CreateAccount() => this.CreateAccount(true);
        public override Account CreateNewAccount(string username)
        {
            var check = GetAccountByUsername(username);
            if (check != null)
                return check;

            var instance = new Account(this);
            instance.Username = username;
            HookAccountEvent(instance);

            var collection = GetAccountCollection<Account>();

            collection.InsertOne(instance);

            return instance;
        }
        internal Account CreateAccount(bool enableEvent=true)
        {
            var account = base.CreateAccount();

            if (enableEvent)
            {
                HookAccountEvent(account);
            }

            return account;
        }
        public override void RemoveAccount(string username)
        {
            var collection = GetAccountCollection<BsonDocument>();

            var filter = Builders<BsonDocument>.Filter.Eq("Username", username);

            collection.DeleteOne(filter);
        }
        public override void SetAccount(Account account)
        {
            var collection = GetAccountCollection<Account>();

            var filter = Builders<Account>.Filter.Eq("Username", account.Username);

            long length = collection.Find(filter).CountDocuments();
            if (length < 1)
                collection.InsertOne(account);
            else
                collection.ReplaceOne(filter, account);
        }
        #endregion

        public override void DeleteAccount(string username)
        {
            var collection = GetAccountCollection<Account>();
            var filter = Builders<Account>
                .Filter
                .Eq("Username", username);

            collection.DeleteMany(filter);
        }

        #region Get Account
        public override Account[] GetAllAccounts()
            => GetAllAccounts(true);
        public Account[] GetAllAccounts(bool hookEvent = true)
        {

            var collection = GetAccountCollection<Account>();
            var result = collection.Find(Builders<Account>.Filter.Where(v => v.Username.Length > 0)).ToList();

            foreach (var item in result)
                item.accountManager = this;
            var accounts = result;
            if (hookEvent)
                foreach (var i in accounts)
                    HookAccountEvent(i);
            return accounts.ToArray();
        }
        public Account GetAccount(string token, bool bumpLastUsed = false, bool hookEvent = true)
        {
            var collection = GetAccountCollection<Account>();

            var filter = Builders<Account>.Filter.Where(v => v.Username.Length > 1);

            var accountList = collection.Find(filter).ToList();
            var response = accountList.Where((v) =>
            {
                return v.Tokens.Where((t) =>
                {
                    return t != null
                        && t.Token != null
                        && t.Token == token
                        && t.Allow;
                }).Count() > 0;
            });

            Account result = response.Count() == 1 ? GetAccountByUsername(response.First().Username, false) : null;

            if (result == null)
                return null;
            result.accountManager = this;
            if (bumpLastUsed)
                TokenUsed(token);
            if (hookEvent)
                HookAccountEvent(result);
            return result;
        }
        public override Account GetAccount(string token, bool bumpLastUsed = false)
            => GetAccount(token, bumpLastUsed, true);
        public override Account GetAccountByUsername(string username)
            => GetAccountByUsername(username, true);
        public Account GetAccountByUsername(string username, bool hookEvent = true)
        {
            var collection = GetAccountCollection<Account>();

            var filter = Builders<Account>
                .Filter
                .Where(v => v.Username == username);
            var result = collection.Find(filter).FirstOrDefault();

            if (result != null)
            {
                if (hookEvent)
                    HookAccountEvent(result);
                result.accountManager = this;
                return result;
            }
            return null;
        }
        public override Account[] GetAccountsByRegex(Regex expression, AccountField field = AccountField.Username)
        {
            var collection = GetAccountCollection<Account>();

            FilterDefinition<Account> filter;
            if (field == AccountField.Username)
            {
                filter = Builders<Account>.Filter.Where(v => expression.Match(v.Username).Success);
            }
            else
            {
                throw new NotImplementedException($"{field} not implemented");
            }
            var result = collection.Find(filter).ToList();

            foreach (var item in result)
            {
                item.accountManager = this;
                HookAccountEvent(item);
            }

            return result.ToArray();
        }
        #endregion


        public override void TokenUsed(string token)
        {
            var account = GetAccount(token, hookEvent: false);
            if (account == null) return;

            var tokenList = new List<AccountToken>();
            for (int i = 0; i < account.Tokens.Length; i++)
            {
                if (account.Tokens[i] != null)
                {
                    tokenList.Add(TokenMarkLastUsedTimestamp(account.Tokens[i]));
                }
            }
            account.accountManager = this;
            account.Tokens = tokenList.ToArray();
        }
        public override bool AccountHasPermission(string token, AccountPermission[] permissions, bool ignoreAdmin = false, bool bumpLastUsed = false)
        {
            var account = GetAccount(token, hookEvent: false);
            if (account == null || !account.enabled) return false;
            if (bumpLastUsed)
                TokenUsed(token);
            account.accountManager = this;
            return AccountHasPermission(account, permissions, ignoreAdmin);
        }
        public override PermissionGroup[] GetPermissionGroups()
        {
            var collection = GetPermissionGroupCollection<PermissionGroup>();
            var filter = Builders<PermissionGroup>
                .Filter
                .Empty;
            var result = collection.Find(filter).ToList();
            return result.ToArray();
        }
        public override PermissionGroup[] GetPermissionGroups(string uid)
        {
            var collection = GetPermissionGroupCollection<PermissionGroup>();
            var filter = Builders<PermissionGroup>
                .Filter
                .Eq("UID", uid);
            var result = collection.Find(filter).ToList();
            return result.ToArray();
        }

        public override void ReadJSON(string jsonContent)
        {
            if (jsonContent.Length < 1)
                Trace.WriteLine($"[AccountManager->Read:{GeneralHelper.GetNanoseconds()}] [ERR] Argument jsonContent has invalid length of {jsonContent.Length}");
            if (!RegExStatements.JSON.Match(jsonContent).Success)
                Trace.WriteLine($"[AccountManager->Read:{GeneralHelper.GetNanoseconds()}] [ERR] Argument jsonContent failed RegExp validation\n--------\n{jsonContent}\n--------\n");
            var jsonOptions = new JsonSerializerOptions()
            {
                IgnoreReadOnlyFields = false,
                IgnoreReadOnlyProperties = false,
                IncludeFields = true
            };
            var deserialized = JsonSerializer.Deserialize<List<Account>>(jsonContent, jsonOptions);
            if (deserialized == null)
            {
                Trace.WriteLine($"[AccountManager->Read:{GeneralHelper.GetNanoseconds()}] [WARN] Deserialized List<Account> is null. Content is\n--------\n{jsonContent}\n--------\n");
            }

            foreach (var item in deserialized)
            {
                var collection = GetAccountCollection<Account>();
                var filter = Builders<Account>.Filter.Where(v => v.Username == item.Username);

                if (collection.Find(filter).ToList().Count < 1)
                    collection.InsertOne(item);
            }
            Console.WriteLine($"[MongoAccountManager->Read] Imported {deserialized.Count} Account(s)");
        }
    }
}
