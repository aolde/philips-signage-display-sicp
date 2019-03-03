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
                var client = new PhilipsSicpClient(socket);
                // client.GetModelInformation();
                // client.GetLedStripState();
                LoopColors(client);
            }
        }

        static void LoopColors(PhilipsSicpClient client)
        {
            var colorProperties = typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.Public);
            var colors = colorProperties.Select(prop => (Color)prop.GetValue(null, null));
            // Color[] colors = new[] { Color.Aqua, Color.Green, Color.Red };
            int index = 0;

            foreach (var color in colors)
            {
                // var color = colors[index % colors.Length];
                Console.WriteLine("Setting color {0}", color);
                client.SetLedStrip(true, color);
                index++;
                Thread.Sleep(1500);
            }
        }
    }
}


