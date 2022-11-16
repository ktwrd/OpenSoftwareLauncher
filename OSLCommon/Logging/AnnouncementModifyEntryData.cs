using JsonDiffPatchDotNet;
using System;
using System.Collections.Generic;
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
            Diff = new Dictionary<object, object[]>();
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
            Diff = JsonSerializer.Deserialize<Dictionary<object, object[]>>(patch.Diff(previousObject, currentObject), options);
        }

        public string AnnouncementId { get; set; }
        public Dictionary<object, object[]> Diff { get; set; }
    }
}
