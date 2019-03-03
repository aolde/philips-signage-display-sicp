using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PhilipsSignageDisplaySicp
{
    public class PhilipsSicpClient
    {
        protected readonly SicpSocket socket;
        protected readonly byte monitorId;
        protected readonly byte groupId;

        public PhilipsSicpClient(SicpSocket socket, byte monitorId = 1, byte groupId = 0)
        {
            this.socket = socket;
            this.monitorId = monitorId;
            this.groupId = groupId;
        }

        public virtual void Set(byte command, params byte[] parameters)
        {
            List<byte> data = new List<byte> { command };
            data.AddRange(parameters);

            socket.Send(new SicpMessage(monitorId, groupId, data.ToArray()));
        }

        public virtual SicpMessage Get(byte command, params byte[] parameters)
        {
            List<byte> data = new List<byte> { command };
            data.AddRange(parameters);

            var responseMessage = socket.Send(new SicpMessage(monitorId, groupId, data.ToArray()));
            return responseMessage;
        }

        public virtual TResult Get<TResult>(byte command, params byte[] parameters) where TResult : ISicpResult, new()
        {
            var responseMessage = Get(command, parameters);
            var result = new TResult();
            
            result.Parse(responseMessage.CommandParameters);
            
            return result;
        }
    }
}


