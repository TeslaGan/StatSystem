using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Core.StatSystem
{
    internal sealed class ReferenceEqualityComparer<T> : IEqualityComparer<T> where T : class
    {
        public static ReferenceEqualityComparer<T> Instance { get; } = new();

        private ReferenceEqualityComparer()
        {
        }

        public bool Equals(T x, T y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}
