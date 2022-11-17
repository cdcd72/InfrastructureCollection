using System.Diagnostics.CodeAnalysis;
using Infra.Core.EventBus.Events;

namespace Infra.Core.EventBus.Abstractions
{
    [SuppressMessage("Naming", "CA1711", Justification = "<Suspended>")]
    public interface IIntegrationEventHandler<in TIntegrationEvent> where TIntegrationEvent : IntegrationEvent
    {
        Task HandleAsync(TIntegrationEvent integrationEvent);
    }
}
