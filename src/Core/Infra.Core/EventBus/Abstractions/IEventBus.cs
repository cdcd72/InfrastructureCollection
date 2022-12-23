using Infra.Core.EventBus.Events;

namespace Infra.Core.EventBus.Abstractions;

public interface IEventBus
{
    void Publish(IntegrationEvent integrationEvent);

    void Subscribe<TIntegrationEvent, TIntegrationEventHandler>()
        where TIntegrationEvent : IntegrationEvent
        where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;

    void SubscribeDynamic<TDynamicIntegrationEventHandler>(string eventName)
        where TDynamicIntegrationEventHandler : IDynamicIntegrationEventHandler;

    void Unsubscribe<TIntegrationEvent, TIntegrationEventHandler>()
        where TIntegrationEvent : IntegrationEvent
        where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;

    void UnsubscribeDynamic<TDynamicIntegrationEventHandler>(string eventName)
        where TDynamicIntegrationEventHandler : IDynamicIntegrationEventHandler;
}