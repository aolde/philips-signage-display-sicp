using System;
using System.Drawing;

namespace PhilipsSignageDisplaySicp.Results
{
    public class SpeakerVolumeResult : ISicpResult
    {
        public double SpeakerVolumePercentage { get; set; }
        public double AudioOutVolumePercentage { get; set; }

        public void Parse(byte[] parameters)
        {
            SpeakerVolumePercentage = parameters[0] / 100d;
            AudioOutVolumePercentage = parameters[1] / 100d;
        }
    }
}