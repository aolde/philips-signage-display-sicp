using System;
using System.Drawing;
using System.Net;
using PhilipsSignageDisplaySicp;

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

                // print hardware and software information
                Console.WriteLine(client.GetPlatformAndModelInfo());

                // turn off the screen
                client.EnableScreen(false);

                // set volume to 50%
                client.SetVolume(0.50);

                // set led strip color to blue
                client.EnableLedStrip(Color.Blue);
            }
        }
    }
}


