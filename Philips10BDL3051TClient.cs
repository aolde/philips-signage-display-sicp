using System;
using System.Drawing;
using PhilipsSignageDisplaySicp.Results;

namespace PhilipsSignageDisplaySicp
{
    // TODO
    // -------------------------------
    // Communication Control V V 0x00
    // Miscellaneous info V 0x0F
    // Serial Code Get V 0x15
    // Power state Set V 0x18 (Screen status only) 🆗
    // Power state Get V 0x19 (Screen status only) 🆗
    // Touch Feature Set V 0x1E
    // Touch Feature Get V 0x1F
    // Power On logo Set V 0x3E
    // Power On logo Get V 0x3F
    // Audio Volume Set V 0x44
    // Audio Volume Get V 0x45
    // Factory Reset Set V 0x56
    // Scheduling Set V 0x5A
    // Scheduling Get V 0x5B
    // Group ID Set V 0x5C
    // Group ID Get V 0x5D
    // Model Number, FW Version, Build date V 0xA1
    // Platform and version labels V 0xA2
    // Input Source V 0xAC
    // Current Source V 0xAD
    // External Storage Lock Set V 0xF1
    // External Storage Lock Get V 0xF2
    // Led Control Set V 0xF3 🆗
    // Led Control Get V 0xF4 🆗

    public class Philips10BDL3051TClient : PhilipsSicpClient
    {
        public Philips10BDL3051TClient(SicpSocket socket, byte monitorId = 1, byte groupId = 0) : base(socket, monitorId, groupId) { }

        public virtual bool IsScreenOn()
        {
            var message = Get(SicpCommands.PowerStateGet);
            return message.CommandParameters[0] == 0x02;
        }

        public virtual void EnableScreen(bool enabled = true)
        {
            Set(SicpCommands.PowerStateSet, enabled.ToByte(trueValue: 0x02, falseValue: 0x01));
        }

        public virtual void EnableLedStrip(Color color)
        {
            Set(SicpCommands.LedControlSet, true.ToByte(), color.R, color.G, color.B);
        }

        public virtual void DisableLedStrip()
        {
            Set(SicpCommands.LedControlSet, false.ToByte(), 0, 0, 0);
        }

        public virtual LedStripResult GetLedStrip()
        {
            return Get<LedStripResult>(SicpCommands.LedControlGet);
        }
    }
}