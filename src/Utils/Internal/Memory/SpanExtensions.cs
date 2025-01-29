// ReSharper disable CheckNamespace

namespace Gabi.Base
{
    internal static class SpanExtensions
    {
#if !NETCOREAPP2_1_OR_GREATER
        public static Span<T> AsSpan<T>(this T[] array)
        {
            return new Span<T>(ref array);
        }

        public static ReadOnlySpan<char> AsSpan(this string str)
        {
            var array = new char[str.Length];
            for (var i = 0; i < str.Length; i++)
                array[i] = str[i];
            return new ReadOnlySpan<char>(ref array);
        }
#endif
    }
}