using BenchmarkDotNet.Attributes;
using System.Collections.Concurrent;

namespace ConcurrentDictionaryBenchmark
{
    [RPlotExporter]
    [MemoryDiagnoser]
    public class GuidKeyBenchmark
    {
        [Params(1000, 10000)]
        public static int TenantSize { get; set; }

        [Params(1000)]
        public static int AcceptedDomainSize { get; set; }


        [Params(10000, 500000)]
        public static int GetOperations { get; set; }


        private static IList<Guid> guidKeys = new List<Guid>();
        private static IList<Guid> nonExistGuidKeys = new List<Guid>();
        private static IList<IList<string>> acceptedDomainsList = new List<IList<string>>();
        private static readonly Random random = new Random();
        private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        [GlobalSetup]
        public void Setup() {
            for (int i = 0; i < TenantSize; i++)
            {
                var tenantId = Guid.NewGuid();
                
                guidKeys.Add(Guid.NewGuid());
                nonExistGuidKeys.Add(Guid.NewGuid());

                var acceptedDomains = new List<string>();
                for (var j = 0; j < 100; j++)
                {
                    var acceptedDomain = $"{RandomString(10)}.com";
                    acceptedDomains.Add(acceptedDomain);
                }

                acceptedDomainsList.Add(acceptedDomains);
            }
        }

        private static string RandomString(int length)
        {
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [Benchmark(Baseline = true)]
        public void WithoutComparer()
        {
            var dict = new ConcurrentDictionary<Guid, ICollection<string>>();
            CommonOps(dict);
        }

        [Benchmark]
        public void WithComparer()
        {
            var dict = new ConcurrentDictionary<Guid, ICollection<string>>(GuidComparer.Instance);
            CommonOps(dict);
        }

        private void CommonOps(ConcurrentDictionary<Guid, ICollection<string>> dict)
        {
            for (var i = 0; i < GetOperations; i++)
            {
                var index = i % TenantSize;
                var acceptedDomains = acceptedDomainsList[index];
                var tenantId = guidKeys[index];
                var nonExistTenantId = nonExistGuidKeys[index];

                dict.TryGetValue(tenantId, out _);
                dict.GetOrAdd(tenantId, acceptedDomains);
                dict.TryGetValue(tenantId, out _);
                dict.TryGetValue(nonExistTenantId, out _);
            }
        }
    }

    /// <summary>
    /// Guid equality comparer
    /// </summary>
    internal class GuidComparer : IEqualityComparer<Guid>
    {
        /// <summary>
        /// Singleton comparer instance
        /// </summary>
        public static readonly GuidComparer Instance = new();

        /// <inheritdoc/>
        public bool Equals(Guid x, Guid y)
        {
            return x.Equals(y);
        }

        /// <inheritdoc/>
        public int GetHashCode(Guid obj)
        {
            return obj.GetHashCode();
        }
    }
}