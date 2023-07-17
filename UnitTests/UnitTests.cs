using ConcurrentDictionaryBenchmark;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class UnitTests
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
