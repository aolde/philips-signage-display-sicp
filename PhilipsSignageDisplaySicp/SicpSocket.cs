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
        private const int COMMUNICATION_CONTROL = 0x00;
        private const int NACK_CODE = 0x15;
        private const int NAV_CODE = 0x18;
        private const int ACK_CODE = 0x06;
        private const int BUFFER_SIZE = 1024;
        private Socket socket;

        /// <summary>
        /// Get or sets the IP address of the monitor.
        /// </summary>
        public IPAddress MonitorIPAddress { get; protected set; }

        /// <summary>
        /// Get or sets the port that the SICP protocol is responding to. Default is 5000.
        /// </summary>
        public int Port { get; protected set; }

        /// <summary>
        /// Gets or sets whether the socket channel should stay connected after a message has been sent and received. 
        /// If false, the socket will connect and disconnect for each call.
        /// </summary>
        public bool KeepAlive { get; protected set; }

        /// <summary>
        /// Handles socket communication with the display.
        /// </summary>
        /// <param name="monitorIPAddress">Get or sets the IP address of the monitor.</param>
        /// <param name="port">Get or sets the port that the SICP protocol is responding to. Default is 5000.</param>
        /// <param name="keepAlive">Gets or sets whether the socket channel should stay connected after a message has been sent and received. If false, the socket will connect and disconnect for each call.</param>
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

                socket.Send(message.ToArray());

                byte[] buffer = new byte[BUFFER_SIZE];
                int bytesReceived = socket.Receive(buffer);

                var responseMessage = SicpMessage.Parse(buffer, bytesReceived);
                var responseData = responseMessage.Data;

                if (responseData[0] == COMMUNICATION_CONTROL)
                {
                    if (responseMessage.Data[1] == NACK_CODE)
                    {
                        throw new SicpNotAcknowledgedException();
                    }
                    else if (responseMessage.Data[1] == NAV_CODE)
                    {
                        throw new SicpNotAvailableException();
                    }
                    else if (responseMessage.Data[1] == ACK_CODE)
                    {
                        return responseMessage;
                    }
                }

                return responseMessage;
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


