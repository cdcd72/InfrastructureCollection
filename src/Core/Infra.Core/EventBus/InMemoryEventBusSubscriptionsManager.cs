using System;
using System.Collections.Generic;
using System.Linq;
using Infra.Core.EventBus.Abstractions;
using Infra.Core.EventBus.Events;
using Infra.Core.EventBus.Models;

namespace Infra.Core.EventBus
{
    public class InMemoryEventBusSubscriptionsManager : IEventBusSubscriptionsManager
    {
        private readonly List<Type> _eventTypes;
        private readonly Dictionary<string, List<SubscriptionInfo>> _handlers;

        #region Constructor

        public InMemoryEventBusSubscriptionsManager()
        {
            _eventTypes = new List<Type>();
            _handlers = new Dictionary<string, List<SubscriptionInfo>>();
        }

        #endregion

        public bool IsEmpty => !_handlers.Keys.Any();

        public event EventHandler<string> OnEventRemoved;

        public void AddSubscription<TIntegrationEvent, TIntegrationEventHandler>()
            where TIntegrationEvent : IntegrationEvent
            where TIntegrationEventHandler : IIntegrationEventHandler<TIntegrationEvent>
        {
            var eventName = GetEventName<TIntegrationEvent>();

            AddSubscription(typeof(TIntegrationEventHandler), eventName, isDynamic: false);

            if (!_eventTypes.Contains(typeof(TIntegrationEvent)))
                _eventTypes.Add(typeof(TIntegrationEvent));
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
            => _handlers.ContainsKey(eventName);

        public IEnumerable<SubscriptionInfo> GetHandlersForEvent<TIntegrationEvent>() where TIntegrationEvent : IntegrationEvent
            => GetHandlersForEvent(GetEventName<TIntegrationEvent>());

        public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName)
            => _handlers[eventName];

        public string GetEventName<TIntegrationEvent>()
            => typeof(TIntegrationEvent).Name;

        public Type GetEventTypeByName(string eventName)
            => _eventTypes.SingleOrDefault(type => type.Name == eventName);

        public void Clear()
            => _handlers.Clear();

        #region Private Method

        private void AddSubscription(Type handlerType, string eventName, bool isDynamic)
        {
            if (!HasSubscriptionsForEvent(eventName))
                _handlers.Add(eventName, new List<SubscriptionInfo>());

            if (_handlers[eventName].Any(sub => sub.HandlerType == handlerType))
                throw new ArgumentException($"Handler Type {handlerType.Name} already registered for '{eventName}'", nameof(handlerType));

            var subscription =
                isDynamic ? SubscriptionInfo.Dynamic(handlerType) : SubscriptionInfo.Typed(handlerType);

            _handlers[eventName].Add(subscription);
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

            return _handlers[eventName].SingleOrDefault(sub => sub.HandlerType == handlerType);
        }

        private void RemoveHandler(string eventName, SubscriptionInfo subscription)
        {
            if (subscription != null)
            {
                _handlers[eventName].Remove(subscription);

                // None subscriptions situation...
                if (!_handlers[eventName].Any())
                {
                    _handlers.Remove(eventName);

                    var eventType = _eventTypes.SingleOrDefault(type => type.Name == eventName);

                    if (eventType != null)
                        _eventTypes.Remove(eventType);

                    RaiseOnEventRemoved(eventName);
                }
            }
        }

        private void RaiseOnEventRemoved(string eventName)
            => OnEventRemoved?.Invoke(this, eventName);

        #endregion
    }
}
