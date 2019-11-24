using System;

namespace SFX.BitHack.CSharp
{
    /// <summary>
    /// Enumeration denoting the error of accessing a <see cref="BitArray"/>
    /// </summary>
    public enum BitArrayAccessError
    {
        /// <summary>
        /// Accessing the <see cref="BitArray"/> with an index out of the supported range was attempted
        /// </summary>
        IndexOutOfRange = 1,
    }

    /// <summary>
    /// Represents the result of accessing a <see cref="BitArray"/>
    /// </summary>
    public struct BitArrayAccessResult<T>
        where T : struct, IEquatable<T>, IComparable<T>, IComparable
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="error">The optional <see cref="BitArrayAccessError"/></param>
        /// <param name="value">The optional value</param>
        internal BitArrayAccessResult(BitArrayAccessError? error, T? value) =>
            (Error, Value) = (error, value);

        public BitArrayAccessError? Error { get; }
        public T? Value { get; }

        public void Deconstruct(out BitArrayAccessError? error, out T? value) =>
            (error, value) = (Error, Value);

        public static implicit operator T(BitArrayAccessResult<T> x)
        {
            if (x.Error.HasValue)
            {
                switch (x.Error.Value)
                {
                    case BitArrayAccessError.IndexOutOfRange:
                        throw new IndexOutOfRangeException();
                    default:
                        throw new Exception();
                }
            }
            else
            {
                if (!x.Value.HasValue)
                    throw new InvalidCastException();
                else return x.Value.Value;
            }
        }
    }
}
