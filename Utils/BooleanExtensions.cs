namespace PhilipsSignageDisplaySicp
{
    public static class BooleanExtensions
    {
        public static byte ToByte(this bool source)
        {
            return source ? (byte)1 : (byte)0;
        }
    }
}