using BenchmarkDotNet.Running;
using ConcurrentDictionaryBenchmark;
using System.Diagnostics;

namespace Tests
{
    [TestClass]
    public class MemoryCacheVsConcurrentLruTests
    {
        [TestMethod]
        [DataRow(10_000, 0.2, 1_000_000)]
        [DataRow(50_000, 0.2, 1_000_000)]
        [DataRow(10_000, 0.1, 1_000_000)]
        [DataRow(50_000, 0.1, 1_000_000)]
        public void MemoryCacheVsConcurrentLruTest(int cacheSize, double oneSDPercentage, int getOrAddOps)
        {
            var benchmark = new MemoryCacheVsConcurrentLru();
            MemoryCacheVsConcurrentLru.CacheSize = cacheSize;
            MemoryCacheVsConcurrentLru.OneSDPercent = oneSDPercentage;
            MemoryCacheVsConcurrentLru.GetOrAddOperations = getOrAddOps;

            for (var i = 0; i < 5; i++)
            {
                benchmark.Setup();
                Run(benchmark.MemoryCache, nameof(benchmark.MemoryCache), i);
                Run(benchmark.ConcurrentLru, nameof(benchmark.ConcurrentLru), i);
            }
        }

        private void Run(Action runDelegate, string functionName, int run)
        {
            var sw = new Stopwatch();
            sw.Start();
            runDelegate();
            sw.Stop();
            Console.WriteLine($"{functionName} Run#{run + 1}: {sw.ElapsedMilliseconds} ms");
        }
    }
}