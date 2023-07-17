using ConcurrentDictionaryBenchmark;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void MemoryCacheTest()
        {
            var benchmark = new TrieVsDictVsMemCache();
            benchmark.Setup();
            benchmark.MemoryCache();
        }
    }
}