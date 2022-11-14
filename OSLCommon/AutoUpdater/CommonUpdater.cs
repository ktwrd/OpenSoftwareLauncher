
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
