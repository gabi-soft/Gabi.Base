// ReSharper disable CheckNamespace

using System;

#if !NETCOREAPP2_1_OR_GREATER
namespace Gabi.Base
# else
namespace Gabi.Base.IGNORE
#endif
{
#if !NETCOREAPP2_1_OR_GREATER
    public class ReadOnlySpan<T> : Span<T>
#else
    internal class ReadOnlySpan<T> : Span<T>
#endif
    {
        public ReadOnlySpan(ref T[] array) : base(ref array)
        {
        }

        public ReadOnlySpan(ref Span<T> span) : base(ref span)
        {
        }

        public override T this[int index]
        {
            get => base[index];
            set => throw new NotSupportedException();
        }

        public static implicit operator ReadOnlySpan<T>(string str)
        {
            var array = str.ToCharArray();
            if (typeof(T) == typeof(char) && array is T[] tArray) return new ReadOnlySpan<T>(ref tArray);
            throw new BaseException($"String ne peux par ètre converti en ReadOnlySpan<{typeof(T)}>");
        }

        public static implicit operator string(ReadOnlySpan<T> readOnlySpan)
        {
            return readOnlySpan.ToString();
        }

        public static implicit operator ReadOnlySpan<T>(Span<char> span)
        {
            if (typeof(T) == typeof(char) && span.ToArray() is T[] tArray) return new ReadOnlySpan<T>(ref tArray);

            throw new InvalidCastException($"Span<char> ne peut pas être converti en ReadOnlySpan<{typeof(T)}>");
        }
    }
}