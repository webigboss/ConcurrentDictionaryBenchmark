using Microsoft.Substrate.CompactSignalStore.Sdk.Implementation;

namespace Tests
{
    [TestClass]
    public class SignalClientTests
    {
        [TestMethod]
        public void TestSignalClient()
        {
            var clientConfig = new CssSdkClientConfig { ApplicationId = Guid.NewGuid() };
            var signalClient = new SignalClient(clientConfig);
        }
    }
}
