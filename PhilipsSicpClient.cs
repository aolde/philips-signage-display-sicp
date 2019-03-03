using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PhilipsSignageDisplaySicp
{
    public class PhilipsSicpClient
    {
        private readonly IPAddress monitorIPAddress;
        private readonly byte monitorId;
        private readonly byte groupId;

        public PhilipsSicpClient(IPAddress monitorIPAddress, byte monitorId = 1, byte groupId = 0)
        {
            this.monitorIPAddress = monitorIPAddress;
            this.monitorId = monitorId;
            this.groupId = groupId;
        }

        public void GetModelInformation() {
            Send(0xA2, 0x00);
        }

        public void SetPowerState(bool state)
        {
            const byte setPowerStateCommand = 0x18;

            Send(setPowerStateCommand, state ? (byte)0x02 : (byte)0x01);
        }

        public void SetLedStrip(bool enabled, Color color)
        {
            const byte setLedStripCommand = 0xF3;

            Send(setLedStripCommand, enabled ? (byte)1 : (byte)0, color.R, color.G, color.B);
        }

        public void GetLedStripState() {
            const byte getLedStripCommand = 0xF4;

            var message = Send(getLedStripCommand);
        }

        public SicpMessage Send(params byte[] data)
        {
            try
            {
                IPEndPoint remoteEndpoint = new IPEndPoint(monitorIPAddress, 5000);
                Socket socket = new Socket(monitorIPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    socket.Connect(remoteEndpoint);

                    var message = new SicpMessage(this.monitorId, this.groupId, data);
                    int bytesSent = socket.Send(message.ToArray());

                    byte[] buffer = new byte[1024];
                    int bytesReceived = socket.Receive(buffer);
                    
                    var sicpResponseMessage = SicpMessage.Parse(buffer, bytesReceived);
                    
                    Console.WriteLine("Response data HEX: {0}", BitConverter.ToString(sicpResponseMessage.Data, 0, sicpResponseMessage.Data.Length));
                    Console.WriteLine("Response data ASCI: {0}", Encoding.ASCII.GetString(sicpResponseMessage.Data, 0, sicpResponseMessage.Data.Length));

                    if (sicpResponseMessage.Data[1] == 0x06)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Acknowledge (ACK)!");
                        Console.ResetColor();
                    }
                    else if (sicpResponseMessage.Data[1] == 0x15)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("Not Acknowledge (NACK)");
                        Console.ResetColor();
                    }
                    else if (sicpResponseMessage.Data[1] == 0x18)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Not Available (NAV). Command not available, not relevant or cannot execute");
                        Console.ResetColor();
                    }

                    return sicpResponseMessage;
                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }
                finally
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return null;
        }

    }
}


