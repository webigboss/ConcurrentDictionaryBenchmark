using Microsoft.AspNet.OData.Builder;
using Microsoft.OData.Edm;

namespace ConcurrentDictionaryBenchmark
{
    public class GetEdmModelBenchmark
    {
        public void GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();

            builder.EntitySet<Signal>(nameof(Signal))
                .EntityType.HasKey(x => x.Id)
                .Filter(nameof(Signal.StartTime), nameof(Signal.EndTime), nameof(Signal.SignalType)); // TODO: figure out why this doesn't work.

            var model = builder.GetEdmModel();
            var signalEntitySet = model.FindDeclaredEntitySet(nameof(Signal));
        }
    }
}
