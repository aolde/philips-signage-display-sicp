# philips-signage-display-sicp
API client for controlling Philips displays that implement Philips SICP protocol. Built using cross-platform .NET Core.

WORK IN PROGRESS!

## Install

A prerelease package has been published to Nuget: https://www.nuget.org/packages/PhilipsSignageDisplaySicp

```
dotnet add package PhilipsSignageDisplaySicp --version 0.1.0-alpha
```

## Usage

```csharp
var displayIpAddress = IPAddress.Parse("192.168.1.132");

using (var socket = new SicpSocket(displayIpAddress, keepAlive: true)) 
{
    // SICP client for Philips 10BDL3051T (10" Android tablet). 
    // can be used for other Philiips displays as well (except led strip)
    var client = new Philips10BDL3051TClient(socket);

    // print hardware and software information
    Console.WriteLine(client.GetPlatformAndModelInfo());

    // turn off the screen
    client.EnableScreen(false);

    // set volume to 50%
    client.SetVolume(0.50);

    // set led strip color to blue
    client.EnableLedStrip(Color.Blue);
}
```