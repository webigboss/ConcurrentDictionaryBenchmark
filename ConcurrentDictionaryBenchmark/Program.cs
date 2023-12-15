using BenchmarkDotNet.Running;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Substrate.CompactSignalStore.Sdk.Implementation;
using System;
using System.Collections.Concurrent;

namespace ConcurrentDictionaryBenchmark
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //BenchmarkRunner.Run<KeyValuePairBenchmark>();
            //BenchmarkRunner.Run<GuidKeyBenchmark>();
            //BenchmarkRunner.Run<TrieVsDictVsMemCache>();
            //BenchmarkRunner.Run<TrieVsDictVsMemCacheConcurrency>();
            //BenchmarkRunner.Run<MemoryCacheVsConcurrentLru>();
            //BenchmarkRunner.Run<SignalClientBenchmark>();
            //BenchmarkRunner.Run<CachePeerCompare>();

            //int counter = 0;

            //var parallelOptions = new ParallelOptions
            //{
            //    MaxDegreeOfParallelism = 9
            //};

            //var cache = new MemoryCache(new MemoryCacheOptions());

            //Parallel.ForEach(
            //    Enumerable.Range(1, 10),
            //    parallelOptions,
            //    i =>
            //    {
            //        var item = cache.GetOrCreate("test-key", cacheEntry =>
            //        {
            //            cacheEntry.SlidingExpiration = TimeSpan.FromSeconds(10);
            //            return Interlocked.Increment(ref counter);
            //        });

            //        Console.Write($"{item} ");
            //    });

            //Console.WriteLine(Environment.NewLine + "------------------");


            //int factoryMethodCalled = 0;
            //var dict = new ConcurrentDictionary<string, int>();
            //Parallel.For(0, 10, parallelOptions, i =>
            //{
            //    var item = dict.GetOrAdd("test-key", key =>
            //    {
            //        Interlocked.Increment(ref factoryMethodCalled);
            //        return 1;
            //    });
            //    // why always 999999 printed, because GetOrAdd method called ConcurrentDictionary.TryAddInternal, which locks the bucket which makes the GetOrAdd synchronous
            //    Console.Write($"{factoryMethodCalled} "); 
            //});

            //Console.ReadLine();

            var clientConfig = new CssSdkClientConfig { ApplicationId = Guid.NewGuid() };
            var signalClient = new SignalClient(clientConfig);
        }
    }
}
