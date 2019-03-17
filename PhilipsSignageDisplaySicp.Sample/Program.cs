using System;
using System.Drawing;
using System.Net;
using System.Threading;
using PhilipsSignageDisplaySicp;
using PhilipsSignageDisplaySicp.Models;

namespace PhilipsSignageDisplaySicp.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var socket = new SicpSocket(IPAddress.Parse("192.168.1.132"), keepAlive: true);

            Console.CancelKeyPress += (sender, e) =>
            {
                Console.WriteLine("Exiting...");
                socket.Disconnect();
            };

            using (socket)
            {
                var client = new Philips10BDL3051TClient(socket);

                Console.WriteLine("Hardware and software information:");
                Console.WriteLine(client.GetPlatformAndModelInfo() + "\n");

                Console.WriteLine("Serial code: " + client.GetSerialCode());
                Console.WriteLine("Group ID: " + client.GetGroupId());
                Console.WriteLine("Input source: " + client.GetInputSource());
                Console.WriteLine("LED: " + client.GetLedStrip());
                Console.WriteLine("Operating hours: " + client.GetOperatingHours());
                Console.WriteLine("Power on logo: " + client.GetPowerOnLogo());
                Console.WriteLine("Schedule 1: " + client.GetSchedule(SchedulePage.Schedule1));
                Console.WriteLine("Volume: " + client.GetVolume().ToString("P"));
                Console.WriteLine("Screen active: " + client.IsScreenOn());
                Console.WriteLine("Touch enabled: " + client.IsTouchEnabled());
                Console.WriteLine("External ports enabled: " + client.IsExternalPortsEnabled());

                Console.WriteLine("Set schedule for page 1...");
                client.SetSchedule(SchedulePage.Schedule1, new Schedule {
                    Enabled = false,
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(13, 16, 0), // increase these numbers and a NAV exception is thrown from display.. unclear why
                    InputSource = InputSource.Browser,
                    Playlist = Playlist.Playlist1,
                    WorkingDays = WorkingDays.Weekdays
                });

                Console.WriteLine("Set input to browser, then media player...");
                client.SetInputSource(InputSource.Browser);
                Thread.Sleep(2000);
                client.SetInputSource(InputSource.MediaPlayer);
                Thread.Sleep(2000);

                Console.WriteLine("Turn screen off and on...");
                client.EnableScreen(false);
                Thread.Sleep(2000);
                client.EnableScreen(true);

                Console.WriteLine("Set volume to 50%...");
                client.SetVolume(0.50);

                Console.WriteLine("Set led strip color to green and blue...");
                client.EnableLedStrip(Color.Green);
                Thread.Sleep(2000);
                client.EnableLedStrip(Color.Blue);
            }
        }
    }
}


