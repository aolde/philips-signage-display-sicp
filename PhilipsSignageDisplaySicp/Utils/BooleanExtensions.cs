namespace PhilipsSignageDisplaySicp
{
    public static class BooleanExtensions
    {
        public static byte ToByte(this bool source, byte trueValue = 1, byte falseValue = 0)
        {
            return source ? trueValue : falseValue;
        }
    }
}