using System;
using System.Drawing;
using System.Net;
using System.Threading;

namespace PhilipsSignageDisplaySicp
{
    class Program
    {
        static void Main(string[] args)
        {
            var ipAddress = IPAddress.Parse("192.168.1.132");
            var client = new PhilipsSicpClient(ipAddress);

            client.GetModelInformation();
        }

        static void LoopColors(PhilipsSicpClient client)
        {
            Color[] colors = new[] { Color.Aqua, Color.Green, Color.Red };
            int index = 0;

            while (true)
            {
                var color = colors[index % colors.Length];
                Console.WriteLine("Setting color {0}", color);
                client.SetLedStrip(true, color);
                index++;
                Thread.Sleep(1500);
            }
        }
    }
}


