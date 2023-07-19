using BenchmarkDotNet.Running;
using ConcurrentDictionaryBenchmark;
using System.Diagnostics;

namespace Tests
{
    [TestClass]
    public class MemoryCacheVsConcurrentLruTests
    {
        [TestMethod]
        [DataRow(10_000, 0.2, 100_000)]
        //[DataRow(50_000, 0.2)]
        //[DataRow(10_000, 0.1)]
        //[DataRow(50_000, 0.1)]
        public void MemoryCacheVsConcurrentLruTest(int cacheSize, double oneSDPercentage, int getOrAddOps)
        {
            var benchmark = new MemoryCacheVsConcurrentLru();
            MemoryCacheVsConcurrentLru.CacheSize = cacheSize;
            MemoryCacheVsConcurrentLru.OneSDPercent = oneSDPercentage;
            MemoryCacheVsConcurrentLru.GetOrAddOperations = getOrAddOps;
            benchmark.Setup();

            Run(benchmark.MemoryCache, nameof(benchmark.MemoryCache));

            Run(benchmark.ConcurrentLru, nameof(benchmark.ConcurrentLru));
        }

        private void Run(Action runDelegate, string functionName)
        {
            for (var i = 0; i < 1; i++)
            {
                var sw = new Stopwatch();
                sw.Start();
                runDelegate();
                sw.Stop();
                Console.WriteLine($"{functionName} Run#{i + 1}: {sw.ElapsedMilliseconds} ms");
            }
        }
    }
}