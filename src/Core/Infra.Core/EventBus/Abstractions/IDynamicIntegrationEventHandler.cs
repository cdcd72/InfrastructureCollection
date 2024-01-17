#pragma warning disable CA1711

namespace Infra.Core.EventBus.Abstractions;

public interface IDynamicIntegrationEventHandler
{
    Task HandleAsync(dynamic eventData);
}
