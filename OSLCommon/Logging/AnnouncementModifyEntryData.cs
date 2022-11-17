using JsonDiffPatchDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.Json;

namespace OSLCommon.Logging
{
    public class AnnouncementModifyEntryData : BaseEntryData
    {
        public AnnouncementModifyEntryData()
            : base()
        {
            AuditType = AuditType.AnnouncementModify;
            AnnouncementId = "";
            Diff = new AnnouncementEntryDiff();
        }
        public AnnouncementModifyEntryData(SystemAnnouncementEntry previous, SystemAnnouncementEntry current)
            : base()
        {
            AuditType = AuditType.AnnouncementModify;
            AnnouncementId = "";

            var options = new JsonSerializerOptions()
            {
                IgnoreReadOnlyFields = true,
                IgnoreReadOnlyProperties = true,
                IncludeFields = true
            };
            var previousObject = JsonSerializer.Serialize(previous, options);
            var currentObject = JsonSerializer.Serialize(current, options);

            var patch = new JsonDiffPatch();
            Diff = AnnouncementEntryDiff.FromDictionary(JsonSerializer.Deserialize<Dictionary<string, object[]>>(patch.Diff(previousObject, currentObject) ?? "{}", options));
        }
        [Description("Announcement ID")]
        public string AnnouncementId { get; set; }
        [Category("Difference")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public AnnouncementEntryDiff Diff { get; set; }
    }
}
