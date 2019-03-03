using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PhilipsSignageDisplaySicp
{
    public class SicpSocket : IDisposable
    {
        private Socket socket;

        /// <summary>
        /// Get or sets the IP address of the monitor.
        /// </summary>
        public IPAddress MonitorIPAddress { get; protected set; }

        /// <summary>
        /// Get or sets the port that the SICP protocol is responding to.
        /// </summary>
        public int Port { get; protected set; }

        /// <summary>
        /// Gets or sets whether the socket channel should stay connected after a message has been sent and received.
        /// </summary>
        public bool KeepAlive { get; protected set; }

        public SicpSocket(IPAddress monitorIPAddress, int port = 5000, bool keepAlive = false)
        {
            MonitorIPAddress = monitorIPAddress;
            Port = port;
            KeepAlive = keepAlive;
        }

        private void InitializeSocket()
        {
            this.socket = new Socket(MonitorIPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect()
        {
            if (this.socket == null)
            {
                InitializeSocket();
            }

            socket.Connect(MonitorIPAddress, Port);
            Console.WriteLine("Connected to socket");
        }

        public void EnsureConnected()
        {
            if (socket == null || !socket.Connected)
            {
                Connect();
            }
        }

        public void Disconnect()
        {
            if (socket != null)
            {
                if (socket.Connected)
                {
                    socket.Shutdown(SocketShutdown.Both);
                }

                socket.Close();
                socket = null;
                Console.WriteLine("Disconnected from socket");
            }
        }

        public SicpMessage Send(SicpMessage message)
        {
            try
            {
                EnsureConnected();
                
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
            finally
            {
                if (!KeepAlive)
                {
                    Disconnect();
                }
            }
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects).
                }

                // free unmanaged resources (unmanaged objects)
                Disconnect();

                // set large fields to null.
                this.MonitorIPAddress = null;

                disposedValue = true;
            }
        }

        ~SicpSocket()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}


