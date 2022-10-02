using kate.shared.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace OSLCommon.Authorization
{
    public class AccountToken
    {
        public static readonly int TokenLength = 32;

        internal Account parentAccount = null;
        public AccountToken(Account parentAccount)
        {
            this.parentAccount = parentAccount;

            Allow = true;
            Token = GeneralHelper.GenerateToken(TokenLength);
            CreatedTimestamp = 0;
        }
        public AccountToken() : this(null)
        { }

        public bool Allow { get; set; }
        public string Token { get; set; }
        public long CreatedTimestamp { get; set; }

        public static AccountToken FromObject(Account parentAccount, object source)
        {
            var jsonOptions = new JsonSerializerOptions()
            {
                IgnoreReadOnlyFields = false,
                IgnoreReadOnlyProperties = false,
                IncludeFields = true
            };
            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(source, jsonOptions), jsonOptions);
            var instance = new AccountToken(parentAccount);
            if (dict.ContainsKey("Token"))
                instance.Token = dict["Token"].ToString();
            if (dict.ContainsKey("CreatedTimestamp"))
                instance.CreatedTimestamp = long.Parse(dict["CreatedTimestamp"].ToString());
            if (dict.ContainsKey("Allow"))
                instance.Allow = bool.Parse(dict["Allow"].ToString());
            return instance;
        }
    }
}
