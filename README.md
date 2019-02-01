# steeltoe-dotnet-sharing-root-certificate
An example Steeltoe solution for an app on Pivotal Application Services (PAS) sharing a root certificate, with an app hosted outside the PAS platform.

### NET-core2_1-linux-valid-certificate
This is a .NET Core application for linux, running on Pivotal Application Services. If you look in the NET-core2_1-linux-valid-certificate.csproj file, you see how this combination is achieved.

Also have a look at the manifest.yml to see how the app is deployed on PAS.

### Scripts
There are the powershell scripts to export an existing root certificate.
