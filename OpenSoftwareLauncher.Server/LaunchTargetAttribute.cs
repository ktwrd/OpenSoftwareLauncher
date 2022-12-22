using System;

namespace OpenSoftwareLauncher.Server
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class LaunchTargetAttribute : Attribute
    {
        public string Name { get; set; }
        public LaunchTargetAttribute(string name)
        {
            Name = name;
        }
    }
}
