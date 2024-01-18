using Infra.Core.EventBus.Abstractions;
using Infra.Core.EventBus.Events;
using Infra.Core.EventBus.Models;

namespace Infra.Core.EventBus;

public class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionsManager
{
    private readonly List<Type> eventTypes = [];
    private readonly Dictionary<string, List<SubscriptionInfo>> handlers = new();

    #region Properties

    public bool IsEmpty => handlers.Keys.Count == 0;

    public event EventHandler<string> OnEventRemoved;

    #endregion

    public void AddSubscription<TIntegrationEvent, TIntegrationEventHandler>()
        where TIntegrationEvent : IntegrationEvent
        where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
    {
        var eventName = GetEventName<TIntegrationEvent>();

        AddSubscription(typeof(TIntegrationEventHandler), eventName, isDynamic: false);

        if (!eventTypes.Contains(typeof(TIntegrationEvent)))
            eventTypes.Add(typeof(TIntegrationEvent));
    }

    public void AddDynamicSubscription<TDynamicIntegrationEventHandler>(string eventName) where TDynamicIntegrationEventHandler : IDynamicIntegrationEventHandler
        => AddSubscription(typeof(TDynamicIntegrationEventHandler), eventName, isDynamic: true);

    public void RemoveSubscription<TIntegrationEvent, TIntegrationEventHandler>()
        where TIntegrationEvent : IntegrationEvent
        where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
    {
        var eventName = GetEventName<TIntegrationEvent>();
        var subscription = FindSubscription<TIntegrationEvent, TIntegrationEventHandler>();

        RemoveHandler(eventName, subscription);
    }

    public void RemoveDynamicSubscription<TDynamicIntegrationEventHandler>(string eventName) where TDynamicIntegrationEventHandler : IDynamicIntegrationEventHandler
    {
        var subscription = FindDynamicSubscription<TDynamicIntegrationEventHandler>(eventName);

        RemoveHandler(eventName, subscription);
    }

    public bool HasSubscriptionsForEvent<TIntegrationEvent>() where TIntegrationEvent : IntegrationEvent
        => HasSubscriptionsForEvent(GetEventName<TIntegrationEvent>());

    public bool HasSubscriptionsForEvent(string eventName)
        => handlers.ContainsKey(eventName);

    public IEnumerable<SubscriptionInfo> GetHandlersForEvent<TIntegrationEvent>() where TIntegrationEvent : IntegrationEvent
        => GetHandlersForEvent(GetEventName<TIntegrationEvent>());

    public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName)
        => handlers[eventName];

    public string GetEventName<TIntegrationEvent>()
        => typeof(TIntegrationEvent).Name;

    public Type GetEventTypeByName(string eventName)
        => eventTypes.SingleOrDefault(type => type.Name == eventName);

    public void Clear()
        => handlers.Clear();

    #region Private Method

    private void AddSubscription(Type handlerType, string eventName, bool isDynamic)
    {
        if (!HasSubscriptionsForEvent(eventName))
            handlers.Add(eventName, new List<SubscriptionInfo>());

        if (handlers[eventName].Any(sub => sub.HandlerType == handlerType))
            throw new ArgumentException($"Handler Type {handlerType.Name} already registered for '{eventName}'", nameof(handlerType));

        var subscription =
            isDynamic ? SubscriptionInfo.Dynamic(handlerType) : SubscriptionInfo.Typed(handlerType);

        handlers[eventName].Add(subscription);
    }

    private SubscriptionInfo FindDynamicSubscription<TDynamicIntegrationEventHandler>(string eventName)
        where TDynamicIntegrationEventHandler : IDynamicIntegrationEventHandler
        => FindSubscription(eventName, typeof(TDynamicIntegrationEventHandler));

    private SubscriptionInfo FindSubscription<TIntegrationEvent, TIntegrationEventHandler>()
        where TIntegrationEvent : IntegrationEvent
        where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
    {
        var eventName = GetEventName<TIntegrationEvent>();

        return FindSubscription(eventName, typeof(TIntegrationEventHandler));
    }

    private SubscriptionInfo FindSubscription(string eventName, Type handlerType)
    {
        if (!HasSubscriptionsForEvent(eventName))
            return null;

        return handlers[eventName].SingleOrDefault(sub => sub.HandlerType == handlerType);
    }

    private void RemoveHandler(string eventName, SubscriptionInfo subscription)
    {
        if (subscription == null) return;

        handlers[eventName].Remove(subscription);

        // None subscriptions situation...
        if (handlers[eventName].Count == 0)
        {
            handlers.Remove(eventName);

            var eventType = eventTypes.SingleOrDefault(type => type.Name == eventName);

            if (eventType != null)
                eventTypes.Remove(eventType);

            RaiseOnEventRemoved(eventName);
        }
    }

    private void RaiseOnEventRemoved(string eventName)
        => OnEventRemoved?.Invoke(this, eventName);

    #endregion
}
