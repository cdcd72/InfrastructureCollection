using Infra.Core.EventBus;
using Infra.Core.IntegrationTest.EventBus.Events;
using Infra.Core.IntegrationTest.EventBus.Handlers;
using NUnit.Framework;

namespace Infra.Core.IntegrationTest.EventBus;

public class InMemoryEventBusSubscriptionsManagerTests
{
    [Test]
    public void AfterCreatedShouldBeEmpty()
    {
        // Arrange
        var manager = new InMemoryEventBusSubscriptionsManager();

        // Act
        var result = manager.IsEmpty;

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void AfterOneEventSubscriptionShouldContainTheEvent()
    {
        // Arrange
        var manager = new InMemoryEventBusSubscriptionsManager();

        // Act
        manager.AddSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();

        // Assert
        Assert.That(manager.HasSubscriptionsForEvent<TestIntegrationEvent>(), Is.True);
    }

    [Test]
    public void AfterAllSubscriptionsDeletedEventShouldNoLongerExists()
    {
        // Arrange
        var manager = new InMemoryEventBusSubscriptionsManager();
        manager.AddSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();

        // Act
        manager.RemoveSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();

        // Assert
        Assert.That(manager.HasSubscriptionsForEvent<TestIntegrationEvent>(), Is.False);
    }

    [Test]
    public void DeletingLastSubscriptionShouldRaiseOnDeletedEvent()
    {
        // Arrange
        var raised = false;
        var manager = new InMemoryEventBusSubscriptionsManager();
        manager.OnEventRemoved += (_, _) => raised = true;
        manager.AddSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();

        // Act
        manager.RemoveSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();

        // Assert
        Assert.That(raised, Is.True);
    }

    [Test]
    public void GetHandlersForEventShouldReturnAllHandlers()
    {
        // Arrange
        var manager = new InMemoryEventBusSubscriptionsManager();
        manager.AddSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();
        manager.AddSubscription<TestIntegrationEvent, TestIntegrationOtherEventHandler>();

        // Act
        var handlers = manager.GetHandlersForEvent<TestIntegrationEvent>();

        // Assert
        Assert.That(handlers.Count(), Is.EqualTo(2));
    }

    [Test]
    public void AfterClearedAllSubscriptionsShouldNoLongerExists()
    {
        // Arrange
        var manager = new InMemoryEventBusSubscriptionsManager();
        manager.AddSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();
        manager.AddSubscription<TestIntegrationEvent, TestIntegrationOtherEventHandler>();

        // Act
        manager.Clear();

        // Assert
        Assert.That(manager.IsEmpty, Is.True);
    }

    [Test]
    public void GetEventTypeByName()
    {
        // Arrange
        var manager = new InMemoryEventBusSubscriptionsManager();
        manager.AddSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();
        var eventName = manager.GetEventName<TestIntegrationEvent>();

        // Act
        var eventType = manager.GetEventTypeByName(eventName);

        // Assert
        Assert.That(eventType.Name, Is.EqualTo(eventName));
    }

    [Test]
    public void RemoveSubscriptionButEventHasNotSubscriptions()
    {
        // Arrange
        var manager = new InMemoryEventBusSubscriptionsManager();

        // Act
        manager.RemoveSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();

        // Assert
        Assert.That(manager.HasSubscriptionsForEvent<TestIntegrationEvent>(), Is.False);
    }

    [Test]
    public void OneEventCanNotAddSameSubscription()
    {
        // Arrange
        var manager = new InMemoryEventBusSubscriptionsManager();

        // Act
        manager.AddSubscription<TestIntegrationEvent, TestIntegrationEventHandler>();

        // Assert
        Assert.Throws<ArgumentException>(()
            => manager.AddSubscription<TestIntegrationEvent, TestIntegrationEventHandler>());
    }

    [Test]
    public void DynamicVersionAfterOneEventSubscriptionShouldContainTheEvent()
    {
        // Arrange
        var manager = new InMemoryEventBusSubscriptionsManager();
        var eventName = manager.GetEventName<TestIntegrationEvent>();

        // Act
        manager.AddDynamicSubscription<TestDynamicIntegrationEventHandler>(eventName);

        // Assert
        Assert.That(manager.HasSubscriptionsForEvent(eventName), Is.True);
    }

    [Test]
    public void DynamicVersionAfterAllSubscriptionsDeletedEventShouldNoLongerExists()
    {
        // Arrange
        var manager = new InMemoryEventBusSubscriptionsManager();
        var eventName = manager.GetEventName<TestIntegrationEvent>();
        manager.AddDynamicSubscription<TestDynamicIntegrationEventHandler>(eventName);

        // Act
        manager.RemoveDynamicSubscription<TestDynamicIntegrationEventHandler>(eventName);

        // Assert
        Assert.That(manager.HasSubscriptionsForEvent(eventName), Is.False);
    }
}