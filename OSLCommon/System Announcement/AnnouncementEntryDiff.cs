using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GenericDifference<string> Message { get; set; }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GenericDifference<long> Timestamp { get; set; }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GenericDifference<bool> Active { get; set; }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GenericDifference<string> ID { get; set; }
        public static AnnouncementEntryDiff FromDictionary(Dictionary<string, object[]> dict)
        {
            var instance = new AnnouncementEntryDiff();
            if (dict.ContainsKey("Message"))
                instance.Message = new GenericDifference<string>
                {
                    Previous = dict["Message"][0].ToString(),
                    Current  = dict["Message"][1].ToString(),
                };
            if (dict.ContainsKey("Timestamp"))
                instance.Timestamp = new GenericDifference<long>
                {
                    Previous = long.Parse(dict["Timestamp"][0].ToString()),
                    Current  = long.Parse(dict["Timestamp"][1].ToString())
                };
            if (dict.ContainsKey("Active"))
                instance.Active = new GenericDifference<bool>
                {
                    Previous = dict["Active"][0].ToString().ToLower() == "true",
                    Current  = dict["Active"][1].ToString().ToLower() == "true",
                };
            if (dict.ContainsKey("ID"))
                instance.ID = new GenericDifference<string>
                {
                    Previous = dict["ID"][0].ToString(),
                    Current  = dict["ID"][1].ToString()
                };
            return instance;
        }
    }
}
