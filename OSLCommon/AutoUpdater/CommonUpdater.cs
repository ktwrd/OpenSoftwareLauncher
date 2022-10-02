using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSLCommon.AutoUpdater
{
    public enum ReleaseStream
    {
        CuttingEdge,
        Beta,
        Stable
    }
    public enum UpdateStates
    {
        NoUpdate,
        Checking,
        Updating,
        Error,
        NeedsRestart,
        Completed,
        EmergencyFallback
    }
}
