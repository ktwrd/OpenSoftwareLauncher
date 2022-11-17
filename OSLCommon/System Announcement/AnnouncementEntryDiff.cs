using System;
using System.Collections.Generic;
using System.Text;

namespace OSLCommon
{
    public class AnnouncementEntryDiff
    {
        public AnnouncementEntryDiff()
        {
            Message = null;
            Timestamp = null;
            Active = null;
            ID = null;
        }
        public GenericDifference<string> Message { get; set; }
        public GenericDifference<long> Timestamp { get; set; }
        public GenericDifference<bool> Active { get; set; }
        public GenericDifference<string> ID { get; set; }
        public static AnnouncementEntryDiff FromDictionary(Dictionary<string, object[]> dict)
        {
            var instance = new AnnouncementEntryDiff();
            if (dict.ContainsKey("Message"))
                instance.Message = new GenericDifference<string>
                {
                    Previous = (string)dict["Message"][0],
                    Current  = (string)dict["Message"][1],
                };
            if (dict.ContainsKey("Timestamp"))
                instance.Timestamp = new GenericDifference<long>
                {
                    Previous = (long)dict["Timestamp"][0],
                    Current  = (long)dict["Timestamp"][1],
                };
            if (dict.ContainsKey("Active"))
                instance.Active = new GenericDifference<bool>
                {
                    Previous = (bool)dict["Active"][0],
                    Current  = (bool)dict["Active"][1],
                };
            if (dict.ContainsKey("ID"))
                instance.ID = new GenericDifference<string>
                {
                    Previous = (string)dict["ID"][0],
                    Current  = (string)dict["ID"][1]
                };
            return instance;
        }
    }
}
