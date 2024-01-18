using Infra.Core.EventBus.Abstractions;

#pragma warning disable CA1711

namespace Infra.Core.IntegrationTest.EventBus.Handlers;

public class TestDynamicIntegrationEventHandler : IDynamicIntegrationEventHandler
{
    public bool Handled { get; private set; }

    public async Task HandleAsync(dynamic eventData)
    {
        Handled = true;

        await Task.CompletedTask;
    }
}
