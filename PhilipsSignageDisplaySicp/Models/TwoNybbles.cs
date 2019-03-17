namespace PhilipsSignageDisplaySicp.Models
{
    public class TwoNybbles
    {
        private readonly byte byteValue;

        public byte High { get => (byte)(byteValue >> 4); }
        public byte Low { get => (byte)(byteValue & 0x0F); }
        public byte Value { get => byteValue; }

        public TwoNybbles(byte high, byte low)
        {
            this.byteValue = (byte)((high << 4) | (low & 0x0F));
        }
    }
}