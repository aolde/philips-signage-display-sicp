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

        public virtual string GetPlatformAndModelInfo()
        {
            var result = new StringBuilder();
            result.Append($"Model {GetModelInfo(ModelInfoField.ModelNumber)}, firmware {GetModelInfo(ModelInfoField.FirmwareVersion)}, build date {GetModelInfo(ModelInfoField.BuildDate)}\n");
            result.Append($"Platform {GetPlatformInfo(PlatformInfoField.PlatformLabel)} (v{GetPlatformInfo(PlatformInfoField.PlatformVersion)}), SICP version {GetPlatformInfo(PlatformInfoField.SICPVersion)}");
            return result.ToString();
        }

        public virtual string GetPlatformInfo(PlatformInfoField field)
        {
            return Get(SicpCommands.PlatformAndVersionLabels, (byte)field).CommandParameters.ToAsciiString();
        }

        public virtual string GetModelInfo(ModelInfoField field)
        {
            return Get(SicpCommands.ModelNumberFwVersionBuildDate, (byte)field).CommandParameters.ToAsciiString();
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


