# philips-signage-display-sicp
API client for controlling Philips displays that implement Philips SICP protocol. Built using cross-platform .NET Core.

---

This project was created to manage a Philips 10BDL3051T display through the Philips SICP protocol. The protocol defines about 50 different commands but only the commands supported by the 10BDL3051T has been implemented (in `Philips10BDL3051TClient`). See instructions below if you require support for a different display. Note that many commands implemented for 10BDL3051T works for other models as well.

Documentation of the SICP protocol can be found online: [Documentation of the SICP protocol, v1.99, 25 May 2017](https://www.keren.nl/dynamic/media/1/documents/Drivers/The%20SICP%20Commands%20Document%20V1_99%2025%20May2017.pdf)

SICP stands for Serial (Ethernet) Interface Communication Protocol.

## Install

A prerelease package has been published to Nuget: https://www.nuget.org/packages/PhilipsSignageDisplaySicp

```
dotnet add package PhilipsSignageDisplaySicp --version 0.1.0-alpha
```

## Usage 

```csharp
var displayIpAddress = IPAddress.Parse("192.168.1.100");

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

See more examples in the [sample console project](https://github.com/aolde/philips-signage-display-sicp/blob/master/PhilipsSignageDisplaySicp.Sample/Program.cs).

## Features implemented for Philips 10BDL3051T

![Philips 10BDL3051T](https://github.com/aolde/philips-signage-display-sicp/raw/master/.github/10bdl3051t.jpg "Philips 10BDL3051T")

The [Philips 10BDL3051T](https://www.philips.co.uk/p-p/10BDL3051T_00/signage-solutions-multi-touch-display) is a 10 inch tablet running Android 4.4, with hardware and software suitable for signage solutions. It is perfect fit for room booking displays or control panels.

The USA market has an almost identical product called [10BDL4151T](https://www.usa.philips.com/p-p/10BDL4151T_00/signage-solutions-multi-touch-display/).

To my knowledge this is the complete feature set implemented by 10BDL3051T. 

- Controlling LED light
  - `EnableLedStrip(Color color)`
  - `DisableLedStrip`
  - `GetLedStrip`
- Toggling screen on and off
  - `IsScreenOn`
  - `EnableScreen(bool enabled = true)`
- Controlling the speaker volume
  - `GetVolume`
  - `SetVolume(double speakerVolumePercentage)`
- Controlling which input/app is active. E.g. browser, media player, PDF player or custom app.
  - `GetInputSource`
  - `SetInputSource(InputSource inputSource)`
- Schedule of when apps/inputs are activated. (*Note: updating of schedule was very buggy in my device*)
  - `GetSchedule(SchedulePage schedule)`
  - `SetSchedule(SchedulePage schedulePage, Schedule schedule)`
- Disabling touch support
  - `IsTouchEnabled`
  - `SetTouchEnabled(bool enabled = true)`
- Setting what is display during boot of the display
  - `GetPowerOnLogo`
  - `SetPowerOnLogo(PowerOnLogo powerOnLogo)`
- Deactivating USB and MicroSD ports to protect from malware and intrusions
  - `IsExternalPortsEnabled`
  - `EnableExternalPorts(bool enabled = true)`
- Control which group the display is part of
  - `GetGroupId`
  - `SetGroupId(byte groupId)`
- Misc. hardware and model information
  - `GetOperatingHours`
  - `GetSerialCode`
  - `GetModelInfo`
  - `GetPlatformInfo`
- Reset the display to factory settings
  - `PerformFactoryReset`

## Implementing a custom display client

If you have a different display than the Philips 10BDL3051T, then you may need to create a custom SICP client. This can be done in the following manner.

1. Create a new class and inherit from `PhilipsSicpClient`:
    ```csharp
    public class CustomPhilipsClient : PhilipsSicpClient {
        public CustomPhilipsClient(SicpSocket socket, byte monitorId = 1, byte groupId = 0) 
            : base(socket, monitorId, groupId) { }
    }
    ```

2. Implement the command methods you need. Read the [documentation](https://www.keren.nl/dynamic/media/1/documents/Drivers/The%20SICP%20Commands%20Document%20V1_99%2025%20May2017.pdf) to know how to format your command parameters. In this example we will implement the "4.1 Power state" command.
    ```csharp
    public virtual bool GetPowerState()
    {
        var message = Get(SicpCommands.PowerStateGet);
        // power is on when first parameter equals to 0x02
        return message.CommandParameters[0] == 0x02;
    }

    public virtual void SetPowerState(bool powerState)
    {
        Set(SicpCommands.PowerStateSet, powerState.ToByte(trueValue: 0x02, falseValue: 0x01));
    }
    ```
3. Instantiate your new client by passing in a SicpSocket instance.
    ```csharp
    var socket = new SicpSocket(IPAddress.Parse("192.168.1.100"));
    var client = new CustomPhilipsClient(socket);

    var powerState = client.GetPowerState();
    client.SetPowerState(!powerState);
    ```

## Philips displays with support of SICP protocol

The following table lists some of the Philips displays that implement the SICP protocol. Every device may support different subsets of the available commands.


| Model                | Platform     |
|----------------------|--------------|
| Philips 10BDL3051T   | Android      |
| Philips 32BDL4050D   | Dragon 1.0   |
| Philips 43BDL4050D   | Dragon 1.0   |
| Philips 43BDL4051T   | Dragon 1.0   |
| Philips 49BDL4050D   | Dragon 1.0   |
| Philips 55BDL4050D   | Dragon 1.0   |
| Philips 55BDL4051T   | Dragon 1.0   |
| Philips 65BDL3051T   | Dragon 1.0   |
| Philips 65BDL4050D   | Dragon 1.0   |
| Philips 42BDL5055P   | Dragon 1.5   |
| Philips 42BDL5057P   | Dragon 1.5   |
| Philips 49BDL5055P   | Dragon 1.5   |
| Philips 49BDL5057P   | Dragon 1.5   |
| Philips 55BDL5055P   | Dragon 1.5   |
| Philips 55BDL5057P   | Dragon 1.5   |
| Philips BDL4676XL    | eagle        |
| Philips BDL4677XL    | eagle        |
| Philips BDL4682XL    | eagle        |
| Philips BDL5585XL    | eagle        |
| Philips BDL5587XL    | eagle        |
| Philips BDL6551V     | eagle        |
| Philips BDL6520EL    | eagle 1.2    |
| Philips BDL6524ET/02 | eagle 1.2    |
| Philips BDL3250EL    | eagle 1.3    |
| Philips BDL4250EL    | eagle 1.3    |
| Philips BDL4252EL    | eagle 1.3    |
| Philips BDL4254ET    | eagle 1.3    |
| Philips BDL4256ET    | eagle 1.3    |
| Philips BDL4271VL    | eagle 1.3    |
| Philips BDL4650EL    | eagle 1.3    |
| Philips BDL4652EL    | eagle 1.3    |
| Philips BDL4671VL    | eagle 1.3    |
| Philips BDL4677XH    | eagle 1.3    |
| Philips BDL4678XL    | eagle 1.3    |
| Philips BDL4776XL    | eagle 1.3    |
| Philips BDL4777XH    | eagle 1.3    |
| Philips BDL4777XL    | eagle 1.3    |
| Philips BDL5551EL    | eagle 1.3    |
| Philips BDL5554ET    | eagle 1.3    |
| Philips BDL5556ET    | eagle 1.3    |
| Philips BDL5571VL    | eagle 1.3    |
| Philips BDL5586XH    | eagle 1.3    |
| Philips BDL5586XL    | eagle 1.3    |
| Philips BDL8470EU    | Himalaya     |
| Philips BDL8470QT    | Himalaya     |
| Philips BDL8470QU    | Himalaya     |
| Philips BDL9870EU    | Himalaya     |
| Philips 75BDL3000U   | Himalaya 1.2 |
| Philips 75BDL3010T   | Himalaya 1.2 |
| Philips 75BDL3003H   | Himalaya 1.2 |
| Philips BDL3220QL    | MTK5580      |
| Philips BDL4220QL    | MTK5580      |
| Philips BDL4235DL    | MTK5580      |
| Philips BDL4620QL    | MTK5580      |
| Philips BDL5520QL    | MTK5580      |
| Philips BDL3230QL    | MTK5580P2    |
| Philips BDL4330QL    | MTK5580P2    |
| Philips BDL4335QL    | MTK5580P2    |
| Philips BDL4830QL    | MTK5580P2    |
| Philips BDL4835QL    | MTK5580P2    |
| Philips BDL5530QL    | MTK5580P2    |
| Philips BDL5535QL    | MTK5580P2    |
| Philips 55BDL1005X   | Phoenix 1.0  |
| Philips 55BDL1007X   | Phoenix 1.0  |
| Philips 65BDL3000Q   | Phoenix 1.0  |
| Philips 65BDL3010T   | Phoenix 1.0  |
| Philips BDL3260EL    | Phoenix 1.0  |
| Philips BDL4260EL    | Phoenix 1.0  |
| Philips BDL4280VL    | Phoenix 1.0  |
| Philips BDL4660EL    | Phoenix 1.0  |
| Philips BDL4680VL    | Phoenix 1.0  |
| Philips BDL4765EL    | Phoenix 1.0  |
| Philips BDL4780VH    | Phoenix 1.0  |
| Philips BDL4988XC    | Phoenix 1.0  |
| Philips BDL4988XL    | Phoenix 1.0  |
| Philips BDL5560EL    | Phoenix 1.0  |
| Philips BDL5580VL    | Phoenix 1.0  |
| Philips BDL5588XC    | Phoenix 1.0  |
| Philips BDL5588XH    | Phoenix 1.0  |
| Philips BDL5588XL    | Phoenix 1.0  |
| Philips BDL6520QL    | Phoenix 1.0  |
| Philips BDL6526QT    | Phoenix 1.0  |
| Philips BDL4270EL    | Phoenix 2.0  |
| Philips BDL4290VL    | Phoenix 2.0  |
| Philips BDL4970EL    | Phoenix 2.0  |
| Philips BDL4990VL    | Phoenix 2.0  |
| Philips BDL5570EL    | Phoenix 2.0  |
| Philips BDL5590VL    | Phoenix 2.0  |
| Philips 55BDL9018L   | LED          |
| Philips 55BDL9025L   | LED          |
