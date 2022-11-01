using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ZstdSharp.Unsafe;

namespace OSLCommon.Authorization
{
    public class MongoAccountManagerCollections
    {
        public string Accounts { get; set; } = "";
    }
    public class MongoAccountManager : AccountManager
    {
        public MongoClient mongoClient;
        public string DatabaseName = "opensoftwarelauncher";
        public MongoAccountManagerCollections Collections = new MongoAccountManagerCollections();
        public MongoAccountManager(MongoClient client)
            : base()
        {
            mongoClient = client;
            AccountUpdated += (eventAccount) =>
            {
                var db = mongoClient.GetDatabase(DatabaseName);
                var collection = db.GetCollection<Account>(Collections.Accounts);

                var filter = Builders<Account>.Filter.Where(v => v.Username == eventAccount.Username);

                collection.ReplaceOne(filter, eventAccount);
            };
        }
        internal void HookAccountEvent(Account account)
        {
            AccountUpdated += (eventAccount) =>
            {
                if (eventAccount.Username == account.Username)
                {
                    account.Merge(eventAccount);
                }
            };
        }

        #region Account Boilerplate

        internal override Account CreateAccount() => this.CreateAccount(true);
        internal override Account CreateNewAccount(string username)
        {
            var check = GetAccountByUsername(username);
            if (check != null)
                return check;

            var instance = new Account(this);
            instance.Username = username;

            var db = mongoClient.GetDatabase(DatabaseName);
            var collection = db.GetCollection<Account>(Collections.Accounts);

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
            var db = mongoClient.GetDatabase(DatabaseName);
            var collection = db.GetCollection<BsonDocument>(Collections.Accounts);

            var filter = Builders<BsonDocument>.Filter.Eq("Username", username);

            collection.DeleteOne(filter);
        }
        public override void SetAccount(Account account)
        {
            var db = mongoClient.GetDatabase(DatabaseName);
            var collection = db.GetCollection<Account>(Collections.Accounts);

            var filter = Builders<Account>.Filter.Eq("Username", account.Username);

            collection.ReplaceOne(filter, account);
        }
        #endregion


        #region Get Account
        public override Account[] GetAllAccounts()
        {
            var db = mongoClient.GetDatabase(DatabaseName);
            var collection = db.GetCollection<Account>(Collections.Accounts);
            var filter = Builders<Account>.Filter.Where(v => v.Username.Length > 1);
            var result = collection.Find(filter).ToList();

            foreach (var item in result)
                item.accountManager = this;
            return result.ToArray();
        }
        public Account[] GetAllAccounts(bool hookEvent = true)
        {
            var accounts = GetAllAccounts();
            if (hookEvent)
                foreach (var i in accounts)
                    HookAccountEvent(i);
            return accounts;
        }
        public Account GetAccount(string token, bool bumpLastUsed = false, bool hookEvent = true)
        {
            var db = mongoClient.GetDatabase(DatabaseName);
            var collection = db.GetCollection<Account>(Collections.Accounts);

            var filter = Builders<Account>.Filter.Where(v => v.HasToken(token));
            var result = collection.Find(filter).FirstOrDefault();

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
            => GetAccount(token, bumpLastUsed);
        public override Account GetAccountByUsername(string username)
        {
            var db = mongoClient.GetDatabase(DatabaseName);
            var collection = db.GetCollection<BsonDocument>(Collections.Accounts);

            var filter = Builders<BsonDocument>.Filter.Eq("Username", username);
            var result = collection.Find(filter).FirstOrDefault();

            var deser = BsonSerializer.Deserialize<Account>(result);
            if (deser != null)
            {
                HookAccountEvent(deser);
                deser.accountManager = this;
                return deser;
            }
            return null;
        }
        public override Account[] GetAccountsByRegex(Regex expression, AccountField field = AccountField.Username)
        {
            var db = mongoClient.GetDatabase(DatabaseName);
            var collection = db.GetCollection<Account>(Collections.Accounts);

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
    }
}
