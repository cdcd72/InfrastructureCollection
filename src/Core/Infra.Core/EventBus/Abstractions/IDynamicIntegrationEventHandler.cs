using System.Diagnostics.CodeAnalysis;

namespace Infra.Core.EventBus.Abstractions
{
    [SuppressMessage("Naming", "CA1711", Justification = "<Suspended>")]
    public interface IDynamicIntegrationEventHandler
    {
        Task HandleAsync(dynamic eventData);
    }
}
