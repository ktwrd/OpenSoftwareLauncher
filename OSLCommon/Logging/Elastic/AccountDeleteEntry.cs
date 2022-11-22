using System;
using System.Collections.Generic;
using System.Text;

namespace OSLCommon.Logging.Elastic
{
    public class AccountDeleteEntry : BaseElasticEntry
    {
        public AccountDeleteEntry()
            : base()
        {
            TargetUsername = "<none>";
        }
        public string TargetUsername { get; set; }
    }
}
