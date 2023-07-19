using System.Collections.Concurrent;

namespace ConcurrentDictionaryBenchmark
{
    internal class CssMemoryCache<TKey, TValue> where TKey : notnull, IEquatable<TKey>
    {
        private readonly ConcurrentDictionary<TKey, TValue> _dict;
        private readonly int _size;

        public CssMemoryCache(int size)
        {
            _size = size;
            _dict = new ConcurrentDictionary<TKey, TValue>();
        }

        public void Test()
        {
        }

        internal class DoubleLinkedListNode
        {
            internal DoubleLinkedListNode? prev;
            internal DoubleLinkedListNode? next;
        }
    }
}
