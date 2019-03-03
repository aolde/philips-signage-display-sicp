using System;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;

namespace PhilipsSignageDisplaySicp
{
    class Program
    {
        static void Main(string[] args)
        {
            var socket = new SicpSocket(IPAddress.Parse("192.168.1.132"), 5000, keepAlive: true);
    
            Console.CancelKeyPress += (sender, e) => {
                Console.WriteLine("Exiting...");
                socket.Disconnect();
            };

            using (socket)
            {
                var client = new Philips10BDL3051TClient(socket);
                
                client.EnableLedStrip(Color.Blue);

                var led = client.GetLedStrip();
                Console.WriteLine("Color {0} {1}", led.Enabled, led.Color.Name);

                client.DisableLedStrip();
            }
        }

        static void LoopColors(Philips10BDL3051TClient client)
        {
            var colorProperties = typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.Public);
            var colors = colorProperties.Select(prop => (Color)prop.GetValue(null, null));
            int index = 0;

            foreach (var color in colors)
            {
                Console.WriteLine("Setting color {0}", color);
                client.EnableLedStrip(color);
                index++;
                Thread.Sleep(1500);
            }
        }
    }
}


