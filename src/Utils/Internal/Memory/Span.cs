using System;

// ReSharper disable CheckNamespace

#if !NETCOREAPP2_1_OR_GREATER
namespace Gabi.Base
# else
namespace Gabi.Base.IGNORE
#endif
{
#if !NETCOREAPP2_1_OR_GREATER
    public class Span<T>
#else
    internal class Span<T>
#endif
    {
        protected bool Equals(Span<T> other)
        {
            return Equals(_array, other._array);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Span<T>)obj);
        }

        public override int GetHashCode()
        {
            return _array != null ? _array.GetHashCode() : 0;
        }

        private static readonly T[] _fakeRef = Array.Empty<T>();
        private readonly T[] _array;

        public Span()
        {
            _array = _fakeRef;
        }

        public Span(ref T[] array)
        {
            _array = array ?? _fakeRef;
        }

        public Span(ref Span<T> span)
        {
            _array = span._array;
        }

        public Span(T[] array, int start, int length)
        {
            _array = length > 0 ? new T[length] : _fakeRef;
            Array.Copy(array, start, _array, 0, length);
        }

        public virtual T this[int index]
        {
            get => _array[index];
            set => _array[index] = value;
        }

        public int Length => _array.Length;

        public bool IsEmpty => _array.Length == 0;

        public static Span<T> Empty => default;

        public static bool operator !=(Span<T> left, Span<T> right)
        {
            return !(left == right);
        }

        public static implicit operator Span<T>(T[] array)
        {
            return new Span<T>(ref array);
        }

        public static implicit operator Span<T>(ArraySegment<T> segment)
        {
            return new Span<T>(segment.Array, segment.Offset, segment.Count);
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public ref readonly T GetPinnableReference()
        {
            if (_array.Length != 0)
                return ref _array[0];
            return ref _fakeRef[0];
        }

        public int IndexOf(T v)
        {
            return Array.IndexOf(_array, v);
        }

        public void CopyTo(ref T[] destination)
        {
            Array.Copy(_array, destination, _array.Length);
        }

        public bool TryCopyTo(T[] destination)
        {
            try
            {
                Array.Copy(_array, destination, _array.Length);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool operator ==(Span<T> left, Span<T> right)
        {
            return right?._array == left?._array;
        }

        public override string ToString()
        {
            if (_array is char[] charArray) return new string(charArray);
            return $"System.Span<{typeof(T).Name}>[{_array.Length}]";
        }

        public Span<T> Slice(int start)
        {
            return new Span<T>(_array, start, _array.Length - start);
        }

        public Span<T> Trim()
        {
            var start = 0;
            var end = Length - 1;
            if (this is Span<char> spanChar)
            {
                for (; start < Length; start++)
                    if (!char.IsWhiteSpace(spanChar[start]))
                        break;

                for (; end > start; end--)
                    if (!char.IsWhiteSpace(spanChar[end]))
                        break;
            }

            return Slice(start, end - start + 1);
        }

        public Span<T> Slice(int start, int length)
        {
            return new Span<T>(_array, start, length);
        }

        public int Count(T value)
        {
            var i = 0;
            foreach (var item in _array)
                if (item.Equals(value))
                    i++;
            return i;
        }

        public T[] ToArray()
        {
            var ret = new T[_array.Length];
            _array.CopyTo(ret, 0);
            return ret;
        }

        public ref struct Enumerator
        {
            private readonly Span<T> _span;
            private int _index;

            internal Enumerator(Span<T> span)
            {
                _span = span;
                _index = -1;
            }

            public bool MoveNext()
            {
                var index = _index + 1;
                if (index < _span.Length)
                {
                    _index = index;
                    return true;
                }

                return false;
            }
        }
    }
}