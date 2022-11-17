using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace OSLCommon
{
    public class GenericDifference<T>
    {
        [Description("Previous state")]
        public T Previous { get; set; }
        [Description("Current State")]
        public T Current { get; set; }
    }
}
