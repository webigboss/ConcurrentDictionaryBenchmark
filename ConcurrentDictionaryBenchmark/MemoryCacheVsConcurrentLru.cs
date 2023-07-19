using BenchmarkDotNet.Attributes;
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

        [Params(50_000)]
        public static int CacheSize { get; set; } = 50_000;

        [Params(1_000_000)]
        public static int GetOrAddOperations { get; set; } = 1_000_000;

        [Params(10)]
        public static int ThreadCount { get; set; } = 10;

        public static double OneSDPercent { get; set; } = 0.2;

        //[Params(5)]
        public static int SlidingExpirationInMs { get; set; } = 5;

        [Params(false)]
        public bool EnableStatistics { get; set; } = true;

        public static int TenantSize { get; set; } = 20;

        private static IList<KeyValuePair<Guid, string>> keyValuePairs = new List<KeyValuePair<Guid, string>>();
        private static IList<KeyValuePair<Guid, string>> nonExistKeyValuePairs = new List<KeyValuePair<Guid, string>>();
        private static IList<UserCacheKey> userCacheKeys = new List<UserCacheKey>();
        private static IList<UserCacheKey> nonExistUserCacheKeys = new List<UserCacheKey>();
        private static IList<CssUser> cssUsers = new List<CssUser>();
        //private static IList<CssUserSlim> cssSlimUsers = new List<CssUserSlim>();
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
                var cssUserSlim = new CssUserSlim 
                { 
                    OId = oId, 
                    IdentityResolutionSuccess = true, 
                    IdentityResolutionTimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds(),  
                    RecipientTypeDetails = RecipientTypeDetails.UserMailbox
                };
                keyValuePairs.Add(new KeyValuePair<Guid, string>(tenantId, smtpAddress));
                userCacheKeys.Add(new UserCacheKey { TenantId = tenantId, SmtpAddress = smtpAddress });
                cssUsers.Add(cssUser);
                //cssSlimUsers.Add(cssUserSlim);
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

        public void MemoryCacheWithSizeLimitAndSlidingExpiration()
        {
            var options = new MemoryCacheOptions { SizeLimit = CacheSize };
            var memcache = new MemoryCache(options);

            Parallel.For(0, UserSize, parallelOptions, i =>
            {
                memcache.GetOrCreate(
                    userCacheKeys[i], 
                    entry => {
                        entry.SlidingExpiration = TimeSpan.FromMilliseconds(SlidingExpirationInMs);
                        entry.Size = 1;
                        return cssUsers[i];
                    });
            });

            Parallel.For(0, GetOrAddOperations, parallelOptions, i =>
            {
                var index = i % UserSize;

                memcache.TryGetValue(userCacheKeys[index], out CssUser cssUser);
                //Assert.IsNotNull(cssUser);

                memcache.TryGetValue(nonExistUserCacheKeys[index], out CssUser cssUser2);
                //Assert.AreEqual(default(CssUser), cssUser2);
            });
        }

        //[Benchmark]
        public void MemoryCacheParallel()
        {
            var options = new MemoryCacheOptions 
            { 
                SizeLimit = CacheSize,
                TrackStatistics = EnableStatistics
            };

            var memCache = new MemoryCache(options);

            Parallel.For(0, GetOrAddOperations, parallelOptions, i =>
            {
                var index = getOrAddOperationIdx[i];
                memCache.GetOrCreate(
                    userCacheKeys[index],
                    entry => {
                        //entry.SlidingExpiration = TimeSpan.FromMilliseconds(SlidingExpirationInMs);
                        entry.Size = 1;
                        return cssUsers[index];
                    });
            });

            if (EnableStatistics)
            {
                var statistics = memCache.GetCurrentStatistics();
                if (statistics != null)
                {
                    var hitRate = statistics.TotalHits * 1d / (statistics.TotalHits + statistics.TotalMisses);
                    Console.WriteLine($"MemoryCache stat: Hit Rate: {hitRate * 100}%, Entry Count:{statistics.CurrentEntryCount}, Estimated Size:{statistics.CurrentEstimatedSize}");
                }
                else
                {
                    Console.WriteLine($"MemoryCache stat: null");
                }
            }
        }

        //[Benchmark]
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

            if (EnableStatistics)
            {
                var statistics = memCache.GetCurrentStatistics();
                if (statistics != null)
                {
                    var hitRate = statistics.TotalHits * 1d / (statistics.TotalHits + statistics.TotalMisses);
                    Console.WriteLine($"MemoryCache stat: Hit Rate: {hitRate * 100}%, Entry Count:{statistics.CurrentEntryCount}, Estimated Size:{statistics.CurrentEstimatedSize}");
                }
                else
                {
                    Console.WriteLine($"MemoryCache stat: null");
                }
            }
        }

        //[Benchmark]
        public void MemoryCacheCssUserSlim()
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

            if (EnableStatistics)
            {
                var statistics = memCache.GetCurrentStatistics();
                if (statistics != null)
                {
                    var hitRate = statistics.TotalHits * 1d / (statistics.TotalHits + statistics.TotalMisses);
                    Console.WriteLine($"MemoryCache stat: Hit Rate: {hitRate * 100}%, Entry Count:{statistics.CurrentEntryCount}, Estimated Size:{statistics.CurrentEstimatedSize}");
                }
                else
                {
                    Console.WriteLine($"MemoryCache stat: null");
                }
            }
        }

        [Benchmark]
        public void ConcurrentLruParallel()
        {
            var lruCache = new ConcurrentLru<UserCacheKey, CssUser>(CacheSize);
            Parallel.For(0, GetOrAddOperations, parallelOptions, i =>
            {
                var index = getOrAddOperationIdx[i];
                lruCache.GetOrAdd(
                    userCacheKeys[index],
                    key => {
                        return cssUsers[index];
                    });
            });
        }

        //[Benchmark]
        public void ConcurrentLru()
        {
            var lruCache = new ConcurrentLru<UserCacheKey, CssUser>(CacheSize);
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

            if (EnableStatistics)
            {
                var metrics = lruCache.Metrics.Value;
                
                if (lruCache.Metrics.HasValue)
                {
                    Console.WriteLine($"{nameof(ConcurrentLru)} stat: Hit Rate: {metrics.HitRatio}%, Evicted :{metrics.Evicted}, Size: {lruCache.Count}");
                }
                else
                {
                    Console.WriteLine($"{nameof(ConcurrentLru)} stat: null");
                }
            }
        }
    }
}
