using BenchmarkDotNet.Attributes;
using BitFaster.Caching;
using BitFaster.Caching.Lfu;
using BitFaster.Caching.Lru;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ConcurrentDictionaryBenchmark
{
    [RPlotExporter]
    [MemoryDiagnoser]
    public class MemoryCacheVsConcurrentLru
    {
        [Params(200_000)]
        public static int UserSize { get; set; } = 200_000;

        [Params(10_000, 50_000)]
        public static int CacheSize { get; set; } = 50_000;

        [Params(1_000_000)]
        public static int GetOrAddOperations { get; set; } = 1_000_000;

        [Params(10)]
        public static int ThreadCount { get; set; } = 10;

        public static double OneSDPercent { get; set; } = 0.2;

        //[Params(5)]
        public static int SlidingExpirationInMs { get; set; } = 1000;
        public static int BitFasterLruAbsoluteExpirationInMs { get; set; } = 200;

        [Params(false)]
        public bool EnableStatistics { get; set; } = true;

        public static int TenantSize { get; set; } = 20;

        private static IList<KeyValuePair<Guid, string>> keyValuePairs = new List<KeyValuePair<Guid, string>>();
        private static IList<KeyValuePair<Guid, string>> nonExistKeyValuePairs = new List<KeyValuePair<Guid, string>>();
        private static IList<UserCacheKey> userCacheKeys = new List<UserCacheKey>();
        private static IList<UserCacheKey> nonExistUserCacheKeys = new List<UserCacheKey>();
        private static IList<CssUser> cssUsers = new List<CssUser>();
        private static Guid[] tenantIds;
        private static IList<Tuple<string, CssUser>>[] valuesPerTenant;
        private static ParallelOptions parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = ThreadCount };
        private static int[] getOrAddOperationIdx = new int[GetOrAddOperations];

        [GlobalSetup]
        public void Setup()
        {
            parallelOptions.MaxDegreeOfParallelism = ThreadCount;
            tenantIds = new Guid[TenantSize];
            valuesPerTenant = new IList<Tuple<string, CssUser>>[TenantSize];

            for (var i = 0; i < TenantSize; i++)
            {
                tenantIds[i] = Guid.NewGuid();
            }

            for (int i = 0; i < UserSize; i++)
            {
                var tenantIdx = i % TenantSize;
                var tenantId = tenantIds[tenantIdx];
                var smtpAddress = Utils.GenerateRandomSmtpAddress();
                var oId = Guid.NewGuid();
                var cssUser = new CssUser
                {
                    OId = oId,
                    TenantId = tenantId,
                    RecipientTypeDetails = RecipientTypeDetails.UserMailbox,
                    IdentityResolutionTimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds(),
                    SmtpAddress = smtpAddress,
                    IdentityResolutionSuccess = true
                };

                keyValuePairs.Add(new KeyValuePair<Guid, string>(tenantId, smtpAddress));
                userCacheKeys.Add(new UserCacheKey { TenantId = tenantId, SmtpAddress = smtpAddress });
                cssUsers.Add(cssUser);
                var nonExistTenantId = Guid.NewGuid();
                var nonExistSmtpAddress = Utils.GenerateRandomSmtpAddress();
                nonExistKeyValuePairs.Add(new KeyValuePair<Guid, string>(nonExistTenantId, nonExistSmtpAddress));
                nonExistUserCacheKeys.Add(new UserCacheKey { TenantId = nonExistTenantId, SmtpAddress = nonExistSmtpAddress });

                if (valuesPerTenant[tenantIdx] == null)
                {
                    valuesPerTenant[tenantIdx] = new List<Tuple<string, CssUser>>();
                }

                valuesPerTenant[tenantIdx].Add(Tuple.Create(smtpAddress, cssUser));
            }

            for (var i = 0; i < GetOrAddOperations; i++)
            {
                var userIdx = Utils.GetNormalIndex(UserSize, OneSDPercent);
                Assert.IsTrue(userIdx >= 0 && userIdx < UserSize, $"UserSize={UserSize}, userIdx={userIdx}");
                getOrAddOperationIdx[i] = userIdx;
            }
        }

        [Benchmark(Baseline = true)]
        public void MemoryCache()
        {
            var options = new MemoryCacheOptions
            {
                SizeLimit = CacheSize,
                TrackStatistics = EnableStatistics
            };

            var semaphore = new SemaphoreSlim(ThreadCount);
            var memCache = new MemoryCache(options);

            var tasks = Enumerable.Range(0, GetOrAddOperations).Select(async i =>
            {
                await semaphore.WaitAsync();
                try
                {
                    var index = getOrAddOperationIdx[i];
                    memCache.GetOrCreate(
                        userCacheKeys[index],
                        entry =>
                        {
                            //entry.SlidingExpiration = TimeSpan.FromMilliseconds(SlidingExpirationInMs);
                            entry.Size = 1;
                            return cssUsers[index];
                        });
                }
                finally
                {
                    semaphore.Release();
                }
            });

            Task.WhenAll(tasks).Wait();
            
            PrintStatMemoryCache(memCache, nameof(MemoryCache));
        }

        [Benchmark]
        public void MemoryCacheWithSlidingExpiration()
        {
            var options = new MemoryCacheOptions
            {
                SizeLimit = CacheSize,
                TrackStatistics = EnableStatistics,
                ExpirationScanFrequency = TimeSpan.FromMilliseconds(500)
            };

            var semaphore = new SemaphoreSlim(ThreadCount);
            var memCache = new MemoryCache(options);

            var tasks = Enumerable.Range(0, GetOrAddOperations).Select(async i =>
            {
                await semaphore.WaitAsync();
                try
                {
                    var index = getOrAddOperationIdx[i];
                    memCache.GetOrCreate(
                        userCacheKeys[index],
                        entry =>
                        {
                            entry.SlidingExpiration = TimeSpan.FromMilliseconds(SlidingExpirationInMs);
                            entry.Size = 1;
                            return cssUsers[index];
                        });
                }
                finally
                {
                    semaphore.Release();
                }
            });

            Task.WhenAll(tasks).Wait();

            PrintStatMemoryCache(memCache, nameof(MemoryCacheWithSlidingExpiration));
        }

        [Benchmark]
        public void ConcurrentLru()
        {
            var builder = new ConcurrentLruBuilder<UserCacheKey, CssUser>();
            builder.WithMetrics().WithCapacity(capacity: CacheSize);
            var lruCache = builder.Build();
            var semaphore = new SemaphoreSlim(ThreadCount);

            var tasks = Enumerable.Range(0, GetOrAddOperations).Select(async i =>
            {
                await semaphore.WaitAsync();
                try
                {
                    var index = getOrAddOperationIdx[i];
                    lruCache.GetOrAdd(
                        userCacheKeys[index],
                        key =>
                        {
                            return cssUsers[index];
                        });
                }
                finally
                {
                    semaphore.Release();
                }
            });

            Task.WhenAll(tasks).Wait();

            PrintStatBitFasterCache(lruCache, nameof(ConcurrentLru));
        }

        [Benchmark]
        public void ConcurrentLruWithAbsoluteExpiration()
        {
            var builder = new ConcurrentLruBuilder<UserCacheKey, CssUser>();
            builder.WithMetrics().WithCapacity(capacity: CacheSize)
                .WithExpireAfterWrite(TimeSpan.FromMilliseconds(BitFasterLruAbsoluteExpirationInMs));

            var lruCache = builder.Build();
            var semaphore = new SemaphoreSlim(ThreadCount);

            var tasks = Enumerable.Range(0, GetOrAddOperations).Select(async i =>
            {
                await semaphore.WaitAsync();
                try
                {
                    var index = getOrAddOperationIdx[i];
                    lruCache.GetOrAdd(
                        userCacheKeys[index],
                        key =>
                        {
                            return cssUsers[index];
                        });
                }
                finally
                {
                    semaphore.Release();
                }
            });

            Task.WhenAll(tasks).Wait();

            PrintStatBitFasterCache(lruCache, nameof(ConcurrentLruWithAbsoluteExpiration));
        }

        [Benchmark]
        public void ConcurrentLfu()
        {
            var lfuCache = new ConcurrentLfu<UserCacheKey, CssUser>(CacheSize);
            var semaphore = new SemaphoreSlim(ThreadCount);

            var tasks = Enumerable.Range(0, GetOrAddOperations).Select(async i =>
            {
                await semaphore.WaitAsync();
                try
                {
                    var index = getOrAddOperationIdx[i];
                    lfuCache.GetOrAdd(
                        userCacheKeys[index],
                        key =>
                        {
                            return cssUsers[index];
                        });
                }
                finally
                {
                    semaphore.Release();
                }
            });

            Task.WhenAll(tasks).Wait();

            PrintStatBitFasterCache(lfuCache, nameof(ConcurrentLfu));
        }

        #region print statistics
        private void PrintStatMemoryCache(IMemoryCache memoryCache, string methodName)
        {
            if (EnableStatistics)
            {
                var statistics = memoryCache.GetCurrentStatistics();
                if (statistics != null)
                {
                    var hitRate = statistics.TotalHits * 1d / (statistics.TotalHits + statistics.TotalMisses);
                    Console.WriteLine($"{methodName}: Hit Rate: {hitRate * 100}%, Count:{statistics.CurrentEntryCount}");
                }
                else
                {
                    Console.WriteLine($"{methodName}: null");
                }
            }
        }
        
        private void PrintStatBitFasterCache<K,V>(ICache<K,V> memCache, string methodName)
        {
            if (EnableStatistics)
            {
                var metrics = memCache.Metrics.Value;

                if (memCache.Metrics.HasValue)
                {
                    Console.WriteLine($"{methodName}: Hit Rate: {metrics.HitRatio * 100}%, Count: {memCache.Count}");
                }
                else
                {
                    Console.WriteLine($"{methodName}: null");
                }
            }
        }
        #endregion
    }
}
