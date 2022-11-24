using System;
using System.IO;
using System.Net;

namespace OpenSoftwareLauncher.Server
{
    public static class GeolocationAdapter
    {
        public static string DatabaseLocation => Path.Join(MainClass.DataDirectory, "geolocation.bin");

        public static IP2Location.Component? Component;

        public static void Initialize()
        {
            if (!ServerConfig.GetBoolean("Telemetry", "AddressGeolocation", false))
            {
                CPrint.Debug("[GeolocationAdapter.Initialize] Disabled in ServerConfig (Telemetry.AddressGeolocation is False)");
                return;
            }

            var start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            // If database doesn't exist, then download.
            if (!File.Exists(DatabaseLocation))
            {
                using (WebClient webClient = new())
                {
                    webClient.DownloadFile(@"https://res.kate.pet/IP2LOCATION-LITE-DB1.BIN", DatabaseLocation);
                }
                CPrint.Debug($"[GeolocationAdapter.Initialize] Downloaded IP Address Database");
            }

            Component = new IP2Location.Component();
            try
            {
                Component.Open(DatabaseLocation);
            }
            catch (Exception e)
            {
                CPrint.Error($"[GeolocationAdapter.Initialize] Failed to open Database. {e}");
                Environment.Exit(1);
            }

            CPrint.WriteLine($"[GeolocationAdapter.Initialize] Took {DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - start}ms");
        }
    }
}
