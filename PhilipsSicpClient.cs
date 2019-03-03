using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PhilipsSignageDisplaySicp
{
    public class PhilipsSicpClient
    {
        private readonly SicpSocket socket;
        private readonly byte monitorId;
        private readonly byte groupId;

        public PhilipsSicpClient(SicpSocket socket, byte monitorId = 1, byte groupId = 0)
        {
            this.socket = socket;
            this.monitorId = monitorId;
            this.groupId = groupId;
        }

        public void GetModelInformation()
        {
            socket.Send(new SicpMessage(monitorId, groupId, new byte[] { 0xA2, 0x00 }));
        }

        public void SetPowerState(bool poweredOn)
        {
            const byte setPowerStateCommand = 0x18;

            socket.Send(new SicpMessage(monitorId, groupId, new byte[] { setPowerStateCommand, (poweredOn ? (byte)0x02 : (byte)0x01) }));
        }

        public void SetLedStrip(bool enabled, Color color)
        {
            const byte setLedStripCommand = 0xF3;

            socket.Send(new SicpMessage(monitorId, groupId, new byte[] { setLedStripCommand, enabled ? (byte)1 : (byte)0, color.R, color.G, color.B }));
        }

        public void GetLedStripState()
        {
            const byte getLedStripCommand = 0xF4;

            socket.Send(new SicpMessage(monitorId, groupId, new byte[] { getLedStripCommand }));
        }

        public void Set(byte command, params byte[] parameters)
        {
            // socket.Send(command);
        }

        public void Get(byte command, params byte[] parameters)
        {

        }
    }
}


