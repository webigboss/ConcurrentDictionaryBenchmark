using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Concurrent;

namespace ConcurrentDictionaryBenchmark
{
    [RPlotExporter]
    [MemoryDiagnoser]
    public class TrieVsDictVsMemCache
    {
        [Params(10000, 50000)]
        public static int UserSize { get; set; } = 10000;

        [Params(10000, 100000)]
        public static int GetOperations { get; set; } = 10000;

        [Params(20)]
        public static int TenantSize { get; set; } = 20;

        private static IList<KeyValuePair<Guid, string>> keyValuePairs = new List<KeyValuePair<Guid, string>>();
        private static IList<KeyValuePair<Guid, string>> nonExistKeyValuePairs = new List<KeyValuePair<Guid, string>>();
        private static IList<CssUser> cssUsers = new List<CssUser>();
        private static Guid[] tenantIds;
        private static IList<Tuple<string, CssUser>>[] trieValuesPerTenant;

        [GlobalSetup]
        public void Setup()
        {
            tenantIds = new Guid[TenantSize];
            trieValuesPerTenant = new IList<Tuple<string, CssUser>>[TenantSize];

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
                cssUsers.Add(cssUser);
                nonExistKeyValuePairs.Add(new KeyValuePair<Guid, string>(Guid.NewGuid(), Utils.GenerateRandomSmtpAddress()));

                //trie values
                if (trieValuesPerTenant[tenantIdx] == null)
                {
                    trieValuesPerTenant[tenantIdx] = new List<Tuple<string, CssUser>>();
                }

                trieValuesPerTenant[tenantIdx].Add(Tuple.Create(smtpAddress, cssUser));
            }
        }

        [Benchmark(Baseline = true)]
        public void ConcurrentDictWithComparer()
        {
            var dict = new ConcurrentDictionary<KeyValuePair<Guid, string>, CssUser>(KeyValuePairComparer<Guid, string>.Instance);
            DictCommonOps(dict);
        }

        [Benchmark]
        public void CompressedNgramTrie()
        {
            var triesPerTenant = new CompressedNgramTrie<CssUser>[TenantSize];
            for (var i = 0; i < TenantSize; i++)
            {
                var triesValues = trieValuesPerTenant[i];
                triesPerTenant[i] = CompressedNgramTrie<CssUser>.Create(triesValues);
            }

            for (var i = 0; i < GetOperations; i++)
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
            }
        }

        [Benchmark]
        public void MemoryCache()
        {
            var memcachePerTenant = new MemoryCache[TenantSize];
            for (var i = 0; i < TenantSize; i++)
            {
                memcachePerTenant[i] = new MemoryCache(new MemoryCacheOptions());

                foreach(var tuple in trieValuesPerTenant[i])
                {
                    memcachePerTenant[i].GetOrCreate(tuple.Item1, entry => tuple.Item2);
                    memcachePerTenant[i].Set(tuple.Item1, tuple.Item2);
                }
            }

            for (var i = 0; i < GetOperations; i++)
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
            }
        }

        private void DictCommonOps(ConcurrentDictionary<KeyValuePair<Guid, string>, CssUser> dict)
        {
            for (var i = 0; i < UserSize; i++)
            {
                dict.TryAdd(keyValuePairs[i], cssUsers[i]);
            }

            for (var i = 0; i < GetOperations; i++)
            {
                var index = i % UserSize;
                dict.TryGetValue(nonExistKeyValuePairs[index], out var cssUser1);
                dict.TryGetValue(keyValuePairs[index], out var cssUser2);
            }
        }
    }
}
