using System.Text;

namespace PhilipsSignageDisplaySicp
{
    public static class ByteExtensions
    {
        public static bool ToBool(this byte source)
        {
            return source == 1;
        }

        public static string ToAsciiString(this byte[] source)
        {
            if (source == null)
            {
                return null;
            }

            return Encoding.ASCII.GetString(source);
        }
    }
}