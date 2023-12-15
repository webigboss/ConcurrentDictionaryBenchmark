using ConcurrentDictionaryBenchmark;

namespace Tests
{
    [TestClass]
    public class GetEdmModelTests
    {
        [TestMethod]
        public void GetEdmModelTest()
        {
            var benchmark = new GetEdmModelBenchmark();
            benchmark.GetEdmModel();
        }
    }
}
