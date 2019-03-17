using System;
using System.Collections;
using System.Drawing;
using System.Text;
using PhilipsSignageDisplaySicp.Models;
using PhilipsSignageDisplaySicp.Results;

namespace PhilipsSignageDisplaySicp
{
    // TODO
    // -------------------------------
    // Miscellaneous info V 0x0F (operating hours) 🆗
    // Serial Code Get V 0x15 🆗
    // Power state Set V 0x18 (Screen status only) 🆗
    // Power state Get V 0x19 (Screen status only) 🆗
    // Touch Feature Set V 0x1E 🆗
    // Touch Feature Get V 0x1F 🆗
    // Power On logo Set V 0x3E 🆗
    // Power On logo Get V 0x3F 🆗
    // Audio Volume Set V 0x44 🆗
    // Audio Volume Get V 0x45 🆗
    // Factory Reset Set V 0x56 🆗
    // Scheduling Set V 0x5A  IN PROGRESS
    // Scheduling Get V 0x5B  🆗
    // Group ID Set V 0x5C 🆗
    // Group ID Get V 0x5D 🆗
    // Model Number, FW Version, Build date V 0xA1 🆗
    // Platform and version labels V 0xA2 🆗
    // Input Source V 0xAC 🆗
    // Current Source V 0xAD 🆗
    // External Storage Lock Set V 0xF1 🆗
    // External Storage Lock Get V 0xF2 🆗
    // Led Control Set V 0xF3 🆗
    // Led Control Get V 0xF4 🆗

    public class Philips10BDL3051TClient : PhilipsSicpClient
    {
        public Philips10BDL3051TClient(SicpSocket socket, byte monitorId = 1, byte groupId = 0) : base(socket, monitorId, groupId) { }

        public virtual void PerformFactoryReset()
        {
            Set(SicpCommands.FactoryResetSet);
        }

        /// <summary>
        /// Checks if external ports like USB or MicroSD are enabled.
        /// </summary>
        public virtual bool IsExternalPortsEnabled()
        {
            var isLocked = Get(SicpCommands.ExternalStorageLockGet, 0x00).CommandParameters[0].ToBool();
            return !isLocked;
        }

        /// <summary>
        /// Enable or disable access to external ports like USB or MicroSD inputs.
        /// </summary>
        public virtual void EnableExternalPorts(bool enabled = true)
        {
            Set(SicpCommands.ExternalStorageLockSet, enabled.ToByte(0x00, 0x01));
        }

        public virtual InputSource GetInputSource()
        {
            return (InputSource)Get(SicpCommands.CurrentSource).CommandParameters[0];
        }

        public virtual void SetInputSource(InputSource inputSource)
        {
            byte osdStyle = 1;
            byte muteStyle = 0;

            Set(SicpCommands.InputSource, (byte)inputSource, (byte)inputSource, osdStyle, muteStyle);
        }

        public virtual TimeSpan GetOperatingHours()
        {
            var sicpMessage = Get(SicpCommands.MiscellaneousInfo, 0x02);
            // making an assumtion here that [0] is days and [1] is hours
            return new TimeSpan(sicpMessage.CommandParameters[0], sicpMessage.CommandParameters[1], 0, 0);
        }

        public virtual Schedule GetSchedule(SchedulePage schedule)
        {
            return Get<Schedule>(SicpCommands.SchedulingGet, (byte)schedule);
        }

        public virtual PowerOnLogo GetPowerOnLogo()
        {
            return (PowerOnLogo)Get(SicpCommands.PowerOnLogoGet).CommandParameters[0];
        }

        public virtual void SetPowerOnLogo(PowerOnLogo powerOnLogo)
        {
            Set(SicpCommands.PowerOnLogoSet, (byte)powerOnLogo);
        }

        public virtual string GetSerialCode()
        {
            var message = Get(SicpCommands.SerialCodeGet);
            return message.CommandParameters.ToAsciiString();
        }

        public virtual bool IsTouchEnabled()
        {
            var message = Get(SicpCommands.TouchFeatureGet);
            return message.CommandParameters[0] == 0x01;
        }

        public virtual void SetTouchEnabled(bool enabled = true)
        {
            Set(SicpCommands.TouchFeatureSet, enabled.ToByte());
        }

        public virtual byte GetGroupId()
        {
            return Get(SicpCommands.GroupIDGet).CommandParameters[0];
        }

        public virtual void SetGroupId(byte groupId)
        {
            Set(SicpCommands.GroupIDSet, groupId);
        }

        public virtual double GetVolume()
        {
            var speakerVolumeResult = Get<SpeakerVolumeResult>(SicpCommands.VolumeGet);
            return speakerVolumeResult.SpeakerVolumePercentage;
        }

        public virtual void SetVolume(double speakerVolumePercentage)
        {
            if (speakerVolumePercentage < 0d || speakerVolumePercentage > 1d)
            {
                throw new ArgumentOutOfRangeException(nameof(speakerVolumePercentage), "Speaker volume should be between 0.0 and 1.0. Example 0.5 = 50 %.");
            }

            var volume = (byte)(speakerVolumePercentage * 100);

            // it was not possible to set different values for Audio Out and Speaker volume with 10BDL3051T
            // so setting both to the same value
            Set(SicpCommands.VolumeSet, volume, volume);
        }

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