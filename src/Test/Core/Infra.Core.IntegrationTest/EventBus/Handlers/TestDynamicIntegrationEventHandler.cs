using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Infra.Core.EventBus.Abstractions;

namespace Infra.Core.IntegrationTest.EventBus.Handlers
{
    [SuppressMessage("Naming", "CA1711", Justification = "<Suspended>")]
    public class TestDynamicIntegrationEventHandler : IDynamicIntegrationEventHandler
    {
        public bool Handled { get; private set; }

        public TestDynamicIntegrationEventHandler() => Handled = false;

        public async Task HandleAsync(dynamic eventData)
        {
            Handled = true;

            await Task.CompletedTask;
        }
    }
}
