namespace HowLong.Extensions
{
    public static class StringExtension
    {
        public static bool IsNullOrEmptyOrWhiteSpace(this string @string) => string.IsNullOrEmpty(@string) || string.IsNullOrWhiteSpace(@string);
    }
}
