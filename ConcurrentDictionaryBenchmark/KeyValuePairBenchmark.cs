using BenchmarkDotNet.Attributes;
using System.Collections.Concurrent;

namespace ConcurrentDictionaryBenchmark
{
    [RPlotExporter]
    [MemoryDiagnoser]
    public class KeyValuePairBenchmark
    {
        [Params(1000, 50000)]
        public static int Size { get; set; }

        [Params(1000, 100000)]
        public static int GetOperations { get; set; }

        private static IList<KeyValuePair<Guid, string>> keyValuePairs = new List<KeyValuePair<Guid, string>>();
        private static IList<KeyValuePair<Guid, string>> nonExistKeyValuePairs = new List<KeyValuePair<Guid, string>>();
        private static IList<CssUser> cssUsers = new List<CssUser>();

        [GlobalSetup]
        public void Setup() {

            var tenantSize = 100;
            var tenantIds = new Guid[tenantSize];
            for (var i = 0; i < tenantSize; i++)
            {
                tenantIds[i] = Guid.NewGuid();
            }

            for (int i = 0; i < Size; i++)
            {
                var tenantId = tenantIds[i % tenantSize];
                var smtpAddress = Utils.GenerateRandomSmtpAddress();
                keyValuePairs.Add(new KeyValuePair<Guid, string>(tenantId, smtpAddress));
                cssUsers.Add(
                    new CssUser
                    { 
                        OId = Guid.NewGuid(), 
                        TenantId = tenantId,
                        RecipientTypeDetails = RecipientTypeDetails.UserMailbox,
                        IdentityResolutionTimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds(),
                        SmtpAddress = smtpAddress,
                        IdentityResolutionSuccess = true
                    });
                nonExistKeyValuePairs.Add(new KeyValuePair<Guid, string>(Guid.NewGuid(), Utils.GenerateRandomSmtpAddress()));
            }
        }

        public void WithoutComparer()
        {
            var dict = new ConcurrentDictionary<KeyValuePair<Guid, string>, CssUser>();
            CommonOps(dict);
        }

        [Benchmark]
        public void WithComparer()
        {
            var dict = new ConcurrentDictionary<KeyValuePair<Guid, string>, CssUser>(KeyValuePairComparer<Guid, string>.Instance);
            CommonOps(dict);
        }

        [Benchmark]
        public void WithGuidToStringPairComparer_Ordinal()
        {
            var dict = new ConcurrentDictionary<KeyValuePair<Guid, string>, CssUser>(GuidToStringPairComparer.Ordinal);
            CommonOps(dict);
        }

        [Benchmark]
        public void WithGuidToStringPairComparer_Ordinal_W_Capacity()
        {
            var dict = new ConcurrentDictionary<KeyValuePair<Guid, string>, CssUser>(concurrencyLevel: Environment.ProcessorCount, capacity: 200, comparer: GuidToStringPairComparer.Ordinal);
            CommonOps(dict);
        }

        public void WithGuidToStringPairComparer_OrdinalIgnoreCase()
        {
            var dict = new ConcurrentDictionary<KeyValuePair<Guid, string>, CssUser>(GuidToStringPairComparer.OrdinalIgnoreCase);
            CommonOps(dict);
        }

        public void WithComparerWithHashCombine()
        {
            var dict = new ConcurrentDictionary<KeyValuePair<Guid, string>, CssUser>(KeyValuePairComparerWithHashCombine<Guid, string>.Instance);
            CommonOps(dict);
        }

        [Benchmark]
        public void ConcurrentDictOfDict()
        {
            var dict = new ConcurrentDictionary<Guid, ConcurrentDictionary<string, CssUser>>();

            for (var i = 0; i < Size; i++)
            {
                var tenantId = keyValuePairs[i].Key;
                var smtpAddress = keyValuePairs[i].Value;
                var cssUser = cssUsers[i];
                if (!dict.TryGetValue(tenantId, out var innerDict))
                {
                    innerDict = new ConcurrentDictionary<string, CssUser>();
                    dict.TryAdd(tenantId, innerDict);
                }
                innerDict.TryAdd(smtpAddress, cssUser);
            }

            for (var i = 0; i < GetOperations; i++)
            {
                var index = i % Size;
                var tenantId = nonExistKeyValuePairs[index].Key;
                var smtpAddress = nonExistKeyValuePairs[index].Value;
                if (dict.TryGetValue(tenantId, out var innerDict))
                {
                    innerDict.TryGetValue(smtpAddress, out var cssUser);
                }

                tenantId = keyValuePairs[index].Key;
                smtpAddress = keyValuePairs[index].Value;
                if (dict.TryGetValue(tenantId, out innerDict))
                {
                    innerDict.TryGetValue(smtpAddress, out var cssUser);
                }
            }
        }

        private void CommonOps(ConcurrentDictionary<KeyValuePair<Guid, string>, CssUser> dict)
        {
            for (var i = 0; i < Size; i++)
            {
                dict.TryAdd(keyValuePairs[i], cssUsers[i]);
            }

            for (var i = 0; i < GetOperations; i++)
            {
                var index = i % Size;
                dict.TryGetValue(nonExistKeyValuePairs[index], out var cssUser1);
                dict.TryGetValue(keyValuePairs[index], out var cssUser2);
            }
        }
    }

    /// <summary>
    /// Key value pair equality comparer
    /// </summary>
    /// <typeparam name="T1">First generic type argument</typeparam>
    /// <typeparam name="T2">Second generic type argument</typeparam>
    internal class KeyValuePairComparer<T1, T2> : IEqualityComparer<KeyValuePair<T1, T2>>
        where T1 : IEquatable<T1> where T2 : IEquatable<T2>
    {
        /// <summary>
        /// Singleton comparer instance
        /// </summary>
        public static readonly KeyValuePairComparer<T1, T2> Instance = new();

        /// <inheritdoc/>
        public bool Equals(KeyValuePair<T1, T2> x, KeyValuePair<T1, T2> y)
        {
            return x.Key.Equals(y.Key) && x.Value.Equals(y.Value);
        }

        /// <inheritdoc/>
        public int GetHashCode(KeyValuePair<T1, T2> obj)
        {
            return (obj.Key.GetHashCode() * 33) ^ obj.Value.GetHashCode();
        }
    }

    internal class GuidToStringPairComparer : IEqualityComparer<KeyValuePair<Guid, string>>
    {
        public static readonly GuidToStringPairComparer Ordinal = new(StringComparison.Ordinal);
        public static readonly GuidToStringPairComparer OrdinalIgnoreCase = new(StringComparison.OrdinalIgnoreCase);

        private StringComparison stringComparison;
        public GuidToStringPairComparer(StringComparison stringComparison)
        {
            this.stringComparison = stringComparison;
        }
        public bool Equals(KeyValuePair<Guid, string> x, KeyValuePair<Guid, string> y)
        {
            return x.Key.Equals(y.Key) && x.Value.Equals(y.Value, stringComparison);
        }

        public int GetHashCode(KeyValuePair<Guid, string> pair)
        {
            return HashCode.Combine(pair.Key.GetHashCode(), pair.Value.GetHashCode(stringComparison));
        }
    }

    internal class KeyValuePairComparerWithHashCombine<T1, T2> : IEqualityComparer<KeyValuePair<T1, T2>>
    where T1 : IEquatable<T1> where T2 : IEquatable<T2>
    {
        /// <summary>
        /// Singleton comparer instance
        /// </summary>
        public static readonly KeyValuePairComparer<T1, T2> Instance = new();

        /// <inheritdoc/>
        public bool Equals(KeyValuePair<T1, T2> x, KeyValuePair<T1, T2> y)
        {
            return x.Key.Equals(y.Key) && x.Value.Equals(y.Value);
        }

        /// <inheritdoc/>
        public int GetHashCode(KeyValuePair<T1, T2> obj)
        {
            return HashCode.Combine(obj.Key.GetHashCode(), obj.Value.GetHashCode());
        }
    }
}