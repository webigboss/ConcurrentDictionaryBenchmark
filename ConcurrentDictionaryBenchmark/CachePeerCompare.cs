using BenchmarkDotNet.Attributes;
using BitFaster.Caching.Lru;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcurrentDictionaryBenchmark
{
    [RPlotExporter]
    [MemoryDiagnoser]
    public class CachePeerCompare
    {
        [Params(40_000)]
        public static int UserSize { get; set; } = 200_000;

        [Params(50_000)]
        public static int CacheSize { get; set; } = 50_000;

        [Params(1_000_000)]
        public static int GetOrAddOperations { get; set; } = 1_000_000;

        [Params(4)]
        public static int ThreadCount { get; set; } = 10;

        public static double OneSDPercent { get; set; } = 0.2;

        //[Params(5)]
        public static int SlidingExpirationInMs { get; set; } = 5;

        [Params(false)]
        public bool EnableStatistics { get; set; } = true;

        public static int TenantSize { get; set; } = 20;

        private static IList<UserCacheKey> userCacheKeys = new List<UserCacheKey>(capacity: UserSize);
        private static IList<CssUser> cssUsers = new List<CssUser>(capacity: UserSize);
        private static Guid[] tenantIds;
        private static ParallelOptions parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = ThreadCount };
        private static int[] getOrAddOperationIdx = new int[GetOrAddOperations];

        [GlobalSetup]
        public void Setup()
        {
            parallelOptions.MaxDegreeOfParallelism = ThreadCount;
            tenantIds = new Guid[TenantSize];

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

                userCacheKeys.Add(new UserCacheKey { TenantId = tenantId, SmtpAddress = smtpAddress });
                cssUsers.Add(cssUser);
            }

            for (var i = 0; i < GetOrAddOperations; i++)
            {
                var userIdx = Utils.GetNormalIndex(UserSize, OneSDPercent);
                //Assert.IsTrue(userIdx >= 0 && userIdx < UserSize, $"UserSize={UserSize}, userIdx={userIdx}");
                getOrAddOperationIdx[i] = userIdx;
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
    }
}
