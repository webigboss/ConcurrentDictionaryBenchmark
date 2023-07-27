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

            var runTimes = 3;
            long timeMemoryCache = 0;
            long timeConcurrentLru = 0;
            Console.WriteLine($"CacheSize: {cacheSize}, UserSize: 200000, GetOrAddOps: {getOrAddOps}, OneSDPercentage: {oneSDPercentage}");
            for (var i = 0; i < runTimes; i++)
            {
                benchmark.Setup();
                Console.WriteLine($"Run#{i + 1}");
                timeMemoryCache += Run(benchmark.MemoryCache, nameof(benchmark.MemoryCache), i);
                timeMemoryCache += Run(benchmark.MemoryCacheWithSlidingExpiration, nameof(benchmark.MemoryCacheWithSlidingExpiration), i);
                timeConcurrentLru += Run(benchmark.ConcurrentLru, nameof(benchmark.ConcurrentLru), i);
                timeConcurrentLru += Run(benchmark.ConcurrentLfu, nameof(benchmark.ConcurrentLfu), i);
            }
        }

        private long Run(Action runDelegate, string functionName, int run)
        {
            var sw = new Stopwatch();
            sw.Start();
            runDelegate();
            sw.Stop();
            Console.WriteLine($"{functionName} Run#{run + 1}: {sw.ElapsedMilliseconds} ms");
            return sw.ElapsedMilliseconds;
        }
    }
}