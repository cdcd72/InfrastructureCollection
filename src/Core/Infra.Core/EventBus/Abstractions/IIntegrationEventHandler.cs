using Infra.Core.EventBus.Events;

#pragma warning disable CA1711

namespace Infra.Core.EventBus.Abstractions;

public interface IIntegrationEventHandler<in TIntegrationEvent> where TIntegrationEvent : IntegrationEvent
{
    Task HandleAsync(TIntegrationEvent integrationEvent);
}
