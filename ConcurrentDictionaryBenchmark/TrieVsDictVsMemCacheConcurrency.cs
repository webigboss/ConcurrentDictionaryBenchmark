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
    public class TrieVsDictVsMemCacheConcurrency
    {
        //[Params(10000, 50000)]
        [Params(50000)]
        public static int UserSize { get; set; } = 10000;

        //[Params(10000, 100000)]
        [Params(100000)]
        public static int GetOperations { get; set; } = 10000;

        [Params(20)]
        public static int TenantSize { get; set; } = 20;

        [Params(10)]
        public static int ThreadCount { get; set; } = 10;

        [Params(5)]
        public static int SlidingExpirationInMs { get; set; } = 5;

        private static IList<KeyValuePair<Guid, string>> keyValuePairs = new List<KeyValuePair<Guid, string>>();
        private static IList<KeyValuePair<Guid, string>> nonExistKeyValuePairs = new List<KeyValuePair<Guid, string>>();
        private static IList<UserCacheKey> userCacheKeys = new List<UserCacheKey>();
        private static IList<UserCacheKey> nonExistUserCacheKeys = new List<UserCacheKey>();
        private static IList<CssUser> cssUsers = new List<CssUser>();
        private static Guid[] tenantIds;
        private static IList<Tuple<string, CssUser>>[] valuesPerTenant;
        private static ParallelOptions parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = ThreadCount };

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
                var cssUser = new CssUser
                {
                    OId = Guid.NewGuid(),
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
        }

        [Benchmark(Baseline = true)]
        public void ConcurrentDictWithComparer()
        {
            var dict = new ConcurrentDictionary<KeyValuePair<Guid, string>, CssUser>(KeyValuePairComparer<Guid, string>.Instance);
            DictCommonOps(dict);
        }

        //[Benchmark]
        public void ConcurrentDictWithoutComparer()
        {
            var dict = new ConcurrentDictionary<KeyValuePair<Guid, string>, CssUser>();
            DictCommonOps(dict);
        }


        [Benchmark]
        public void ConcurrentDictWithUserCacheKey()
        {
            var dict = new ConcurrentDictionary<UserCacheKey, CssUser>();
            DictCommonOpsWithUserCacheKey(dict);
        }

        //[Benchmark]
        public void CompressedNgramTrie()
        {
            var triesPerTenant = new CompressedNgramTrie<CssUser>[TenantSize];
            Parallel.For(0, TenantSize, parallelOptions, i =>
            {
                var triesValues = valuesPerTenant[i];
                triesPerTenant[i] = CompressedNgramTrie<CssUser>.Create(triesValues);
            });

            Parallel.For(0, GetOperations, parallelOptions, i =>
            {
                var index = i % UserSize;
                var tenantIdx = index % TenantSize;
                var tenantId = keyValuePairs[index].Key;
                //Assert.IsTrue(tenantId == tenantIds[tenantIdx]);
                var smtpAddress = keyValuePairs[index].Value;

                var cssUsers = triesPerTenant[tenantIdx].Search(smtpAddress);
                //Assert.IsTrue(cssUsers.Any());

                var nonExistSmtpAddress = nonExistKeyValuePairs[index].Value;
                var cssUsers2 = triesPerTenant[tenantIdx].Search(nonExistSmtpAddress);
                //Assert.IsFalse(cssUsers2.Any());
            });
        }

        //[Benchmark]
        public void MemoryCache()
        {
            var memcachePerTenant = new MemoryCache[TenantSize];

            for (var i = 0; i < TenantSize; i++)
            {
                memcachePerTenant[i] = new MemoryCache(new MemoryCacheOptions());
            }

            Parallel.For(0, TenantSize, parallelOptions, i =>
            {
                foreach (var tuple in valuesPerTenant[i])
                {
                    memcachePerTenant[i].GetOrCreate(tuple.Item1, entry => tuple.Item2);
                    //memcachePerTenant[i].Set(tuple.Item1, tuple.Item2);
                }
            });

            Parallel.For(0, GetOperations, parallelOptions, i =>
            {
                var index = i % UserSize;
                var tenantIdx = index % TenantSize;
                var tenantId = keyValuePairs[index].Key;
                //Assert.IsTrue(tenantId == tenantIds[tenantIdx]);
                var smtpAddress = keyValuePairs[index].Value;

                memcachePerTenant[tenantIdx].TryGetValue(smtpAddress, out CssUser cssUser);
                //Assert.IsNotNull(cssUser);

                memcachePerTenant[tenantIdx].TryGetValue(nonExistKeyValuePairs[index].Value, out CssUser cssUser2);
                //Assert.AreEqual(default(CssUser), cssUser2);
            });
        }

        [Benchmark]
        public void MemoryCacheWithSlidingExpiration()
        {
            var memcachePerTenant = new MemoryCache[TenantSize];
            var userCountPerTenant = UserSize / TenantSize;
            var sizeLimit = userCountPerTenant / 2;

            var options = new MemoryCacheOptions { SizeLimit = sizeLimit };

            for (var i = 0; i < TenantSize; i++)
            {
                memcachePerTenant[i] = new MemoryCache(new MemoryCacheOptions());
            }

            Parallel.For(0, TenantSize, parallelOptions, i =>
            {
                foreach (var tuple in valuesPerTenant[i])
                {
                    memcachePerTenant[i].GetOrCreate(
                        tuple.Item1,
                        entry =>
                        {
                            entry.SlidingExpiration = TimeSpan.FromMilliseconds(SlidingExpirationInMs);
                            entry.Size = 1;
                            return tuple.Item2;
                        });
                }
            });

            Parallel.For(0, GetOperations, parallelOptions, i =>
            {
                var index = i % UserSize;
                var tenantIdx = index % TenantSize;
                var tenantId = keyValuePairs[index].Key;
                //Assert.IsTrue(tenantId == tenantIds[tenantIdx]);
                var smtpAddress = keyValuePairs[index].Value;

                memcachePerTenant[tenantIdx].TryGetValue(smtpAddress, out CssUser cssUser);
                //Assert.IsNotNull(cssUser);

                memcachePerTenant[tenantIdx].TryGetValue(nonExistKeyValuePairs[index].Value, out CssUser cssUser2);
                //Assert.AreEqual(default(CssUser), cssUser2);
            });
        }

        [Benchmark]
        public void MemoryCacheNotPerTenantWithSizeLimitAndSlidingExpiration()
        {
            var sizeLimit = UserSize * 2 / 3;
            var options = new MemoryCacheOptions { SizeLimit = sizeLimit };
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

            Parallel.For(0, GetOperations, parallelOptions, i =>
            {
                var index = i % UserSize;

                memcache.TryGetValue(userCacheKeys[index], out CssUser cssUser);
                //Assert.IsNotNull(cssUser);

                memcache.TryGetValue(nonExistUserCacheKeys[index], out CssUser cssUser2);
                //Assert.AreEqual(default(CssUser), cssUser2);
            });
        }

        [Benchmark]
        public void MemoryCacheNotPerTenant()
        {
            var options = new MemoryCacheOptions();
            var memcache = new MemoryCache(options);

            Parallel.For(0, UserSize, parallelOptions, i =>
            {
                memcache.GetOrCreate(
                    userCacheKeys[i],
                    entry => {
                        entry.SlidingExpiration = TimeSpan.FromMilliseconds(SlidingExpirationInMs);
                        return cssUsers[i];
                    });
            });

            Parallel.For(0, GetOperations, parallelOptions, i =>
            {
                var index = i % UserSize;

                memcache.TryGetValue(userCacheKeys[index], out CssUser cssUser);
                //Assert.IsNotNull(cssUser);

                memcache.TryGetValue(nonExistUserCacheKeys[index], out CssUser cssUser2);
                //Assert.AreEqual(default(CssUser), cssUser2);
            });
        }

        [Benchmark]
        public void ConcurrentLru()
        {
            var lruCache = new ConcurrentLru<UserCacheKey, CssUser>(UserSize / 2);
            Parallel.For(0, UserSize, parallelOptions, i =>
            {
                lruCache.GetOrAdd(
                    userCacheKeys[i],
                    key => {
                        return cssUsers[i];
                    });
            });

            Parallel.For(0, GetOperations, parallelOptions, i =>
            {
                var index = i % UserSize;

                lruCache.TryGet(userCacheKeys[index], out CssUser cssUser);
                //Assert.IsNotNull(cssUser);

                lruCache.TryGet(nonExistUserCacheKeys[index], out CssUser cssUser2);
                //Assert.AreEqual(default(CssUser), cssUser2);
            });
        }

        private void DictCommonOps(ConcurrentDictionary<KeyValuePair<Guid, string>, CssUser> dict)
        {
            Parallel.For(0, UserSize, parallelOptions, i =>
            {
                dict.TryAdd(keyValuePairs[i], cssUsers[i]);
            });

            Parallel.For(0, GetOperations, parallelOptions, i =>
            {
                var index = i % UserSize;
                dict.TryGetValue(keyValuePairs[index], out var cssUser2);
                dict.TryGetValue(nonExistKeyValuePairs[index], out var cssUser1);
            });
        }

        private void DictCommonOpsWithUserCacheKey(ConcurrentDictionary<UserCacheKey, CssUser> dict)
        {
            Parallel.For(0, UserSize, parallelOptions, i =>
            {
                dict.TryAdd(userCacheKeys[i], cssUsers[i]);
            });

            Parallel.For(0, GetOperations, parallelOptions, i =>
            {
                var index = i % UserSize;
                dict.TryGetValue(userCacheKeys[index], out var cssUser2);
                //dict.TryGetValue(nonExistKeyValuePairs[index], out var cssUser1);
                dict.TryGetValue(nonExistUserCacheKeys[index], out var cssUser1);
            });
        }
    }
}
