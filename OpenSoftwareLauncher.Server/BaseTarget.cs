namespace OpenSoftwareLauncher.Server
{
    public abstract class BaseTarget
    {
        public IServer Server { get; set; }
        public abstract void Register();
    }
}
