using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Infra.Core.EventBus.Abstractions;
using Infra.Core.IntegrationTest.EventBus.Events;

namespace Infra.Core.IntegrationTest.EventBus.Handlers
{
    [SuppressMessage("Naming", "CA1711", Justification = "<Suspended>")]
    public class TestIntegrationOtherEventHandler : IIntegrationEventHandler<TestIntegrationEvent>
    {
        public bool Handled { get; private set; }

        public TestIntegrationOtherEventHandler() => Handled = false;

        public async Task HandleAsync(TestIntegrationEvent integrationEvent)
        {
            Handled = true;

            await Task.CompletedTask;
        }
    }
}
