# Open Software Launcher
An Open-source software license and version management platform, utilizing ASP.NET Core, MongoDB, and WinForms.

[Latest Release](https://github.com/ktwrd/opensoftwarelauncher/release/latest)

To install the admin client, download the installer that has the filename like `OSLAdmin.YYYYmmDD_HHMM.Update.exe`.

## Installing the Server
Requirements
- MongoDB Server
- .NET 6.0 Runtime (including `aspnetcore` runtime)
- Authentication Provider Server (see [Authentication Provider.md](Authentication%20Provider.md))

Download the latest release for OSL Server (https://github.com/ktwrd/opensoftwarelauncher/release/latest) and execute `OpenSoftwareLauncher.Server`.

After you've executed that, set the full url for the MongoDB server in `Config/config.ini` at `Connection.MongoDBServer` and your authentication provider URL's at `Authentication.Provider` and `Authentication.ProviderSignupURL`.

Once you've done that, you can start the server up again. Since it's the first launch a "superuser" account will be created and it's token will be written to a file named `superuser-token.txt`. You can use this to grant your own account the Administrator permission once you've logged in for the first time.
