using Infra.Core.EventBus.Abstractions;
using Infra.Core.IntegrationTest.EventBus.Events;

#pragma warning disable CA1711

namespace Infra.Core.IntegrationTest.EventBus.Handlers;

public class TestIntegrationEventHandler : IIntegrationEventHandler<TestIntegrationEvent>
{
    public bool Handled { get; private set; }

    public async Task HandleAsync(TestIntegrationEvent integrationEvent)
    {
        Handled = true;

        await Task.CompletedTask;
    }
}
