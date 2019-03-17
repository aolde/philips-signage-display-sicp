using System.Drawing;

namespace PhilipsSignageDisplaySicp.Models
{
    public class LedStripResult : ISicpResult
    {
        public bool Enabled { get; set; }
        public Color Color { get; set; }

        public void Parse(byte[] parameters)
        {
            Enabled = parameters[0].ToBool();
            Color = Color.FromArgb(parameters[1], parameters[2], parameters[3]);
        }

        public override string ToString()
        {
            return $"Enabled: {Enabled}, Color: {Color}";
        }
    }
}