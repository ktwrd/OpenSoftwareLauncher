using Microsoft.AspNetCore.Builder;
using OSLCommon.Helpers;
using System;

namespace OpenSoftwareLauncher.Server
{
    public interface IServer
    {
        event ParameterDelegate<WebApplicationBuilder> AspNetCreate_PreBuild;
        event ParameterDelegate<WebApplication> AspNetCreate_PreRun;
        IServiceProvider Provider { get; }
    }
}
