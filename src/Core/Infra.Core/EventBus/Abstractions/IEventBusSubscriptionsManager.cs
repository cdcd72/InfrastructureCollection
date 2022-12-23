using Infra.Core.EventBus.Events;
using Infra.Core.EventBus.Models;

namespace Infra.Core.EventBus.Abstractions;

public interface IEventBusSubscriptionsManager
{
    bool IsEmpty { get; }

    event EventHandler<string> OnEventRemoved;

    void AddSubscription<TIntegrationEvent, TIntegrationEventHandler>()
        where TIntegrationEvent : IntegrationEvent
        where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;

    void AddDynamicSubscription<TDynamicIntegrationEventHandler>(string eventName)
        where TDynamicIntegrationEventHandler : IDynamicIntegrationEventHandler;

    void RemoveSubscription<TIntegrationEvent, TIntegrationEventHandler>()
        where TIntegrationEvent : IntegrationEvent
        where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>;

    void RemoveDynamicSubscription<TDynamicIntegrationEventHandler>(string eventName)
        where TDynamicIntegrationEventHandler : IDynamicIntegrationEventHandler;

    bool HasSubscriptionsForEvent<TIntegrationEvent>() where TIntegrationEvent : IntegrationEvent;

    bool HasSubscriptionsForEvent(string eventName);

    IEnumerable<SubscriptionInfo> GetHandlersForEvent<TIntegrationEvent>() where TIntegrationEvent : IntegrationEvent;

    IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);

    string GetEventName<TIntegrationEvent>();

    Type GetEventTypeByName(string eventName);

    void Clear();
}