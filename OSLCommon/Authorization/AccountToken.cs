using kate.shared.Helpers;
using System.Collections.Generic;
using System.Security.Cryptography;
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
            UserAgent = "unknown";
            Host = "0.0.0.0";
            CreatedTimestamp = 0;
            LastUsed = 0;
        }
        public AccountToken() : this(parentAccount: null)
        {
            Allow = true;
            Token = GeneralHelper.GenerateToken(TokenLength);
            CreatedTimestamp = 0;
            UserAgent = "unknown";
            Host = "0.0.0.0";
            LastUsed = 0;
        }

        public AccountToken(AccountTokenDetailsResponse res)
        {
            Allow = res.Enabled;
            _tokenHash = res.Hash;
            UserAgent = res.UserAgent;
            Host = res.Host;
            CreatedTimestamp = res.CreatedTimestamp;
            LastUsed = res.LastUsed;
        }

        public bool Allow { get; set; } = true;
        public string Token { get; set; }
        private string _tokenHash = null;
        public string TokenHash
        {
            get
            {
                if (_tokenHash != null) return _tokenHash;
                StringBuilder Sb = new StringBuilder();

                using (var hash = SHA256.Create())
                {
                    Encoding enc = Encoding.UTF8;
                    byte[] result = hash.ComputeHash(enc.GetBytes(Token));

                    foreach (byte b in result)
                        Sb.Append(b.ToString("x2"));
                }

                return Sb.ToString();
            }
        }
        public string UserAgent { get; set; }
        public string Host { get; set; }
        public long CreatedTimestamp { get; set; }
        public long LastUsed { get; set; }

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
            if (dict.ContainsKey("LastUsed"))
                instance.LastUsed = long.Parse(dict["LastUsed"].ToString());
            if (dict.ContainsKey("Host"))
                instance.Host = dict["Host"].ToString();
            if (dict.ContainsKey("UserAgent"))
                instance.UserAgent = dict["UserAgent"].ToString();
            return instance;
        }
    }
}
