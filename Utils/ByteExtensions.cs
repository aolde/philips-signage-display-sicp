namespace PhilipsSignageDisplaySicp
{
    public static class ByteExtensions
    {
        public static bool ToBool(this byte source)
        {
            return source == 1;
        }
    }
}