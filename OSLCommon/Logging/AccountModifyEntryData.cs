using JsonDiffPatchDotNet;
using OSLCommon.Authorization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.Json;

namespace OSLCommon.Logging
{
    public class AccountModifyEntryData : BaseEntryData
    {
        public AccountModifyEntryData()
            : base()
        {
            AuditType = AuditType.AccountModify;
            Username = "";
            Diff = new object();
        }
        public AccountModifyEntryData(Account previous, Account current)
            : base()
        {
            AuditType = AuditType.AccountModify;
            Username = previous.Username;

            var options = new JsonSerializerOptions()
            {
                IgnoreReadOnlyFields = true,
                IgnoreReadOnlyProperties = true,
                IncludeFields = true
            };
            var previousObject = JsonSerializer.Serialize(previous, options);
            var currentObject = JsonSerializer.Serialize(current, options);

            var patch = new JsonDiffPatch();
            Diff = (object)JsonSerializer.Deserialize<Dictionary<string, object[]>>(patch.Diff(previousObject, currentObject) ?? "{}", options);
        }

        public string Username { get; set; }
        [Category("Account Difference (array is before and after)")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public object Diff { get; set; }
    }
}
