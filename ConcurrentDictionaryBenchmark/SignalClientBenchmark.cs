using BenchmarkDotNet.Attributes;
using Microsoft.Substrate.CompactSignalStore.Sdk.Implementation;


namespace ConcurrentDictionaryBenchmark
{
    [MemoryDiagnoser]
    [RPlotExporter]
    public class SignalClientBenchmark
    {
        private readonly Guid applicationId = Guid.NewGuid();

        /// <summary>
        /// BenchmarkSignalClient
        /// </summary>
        [Benchmark]
        public void BenchmarkSignalClient()
        {
            var clientConfig = new CssSdkClientConfig { ApplicationId = applicationId };
            var signalClient = new SignalClient(clientConfig);
        }
    }
}
