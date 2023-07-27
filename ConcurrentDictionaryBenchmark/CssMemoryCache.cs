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

    interface ICssMemoryCache<TKey, TValue> where TKey : notnull, IEquatable<TKey>
    {
        int Count { get; }
        
        bool GetOrAdd(TKey key, Func<TKey, TValue> valueFactory, out TValue value);
        
        bool TryGetValue(TKey key, out TValue value);
    }
}
