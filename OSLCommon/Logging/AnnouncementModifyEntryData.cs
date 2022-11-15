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
            Diff = "";
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
            Diff = patch.Diff(previousObject, currentObject);
        }

        public string AnnouncementId { get; set; }
        public string Diff { get; set; }
    }
}
