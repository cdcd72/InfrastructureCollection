using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Infra.Core.EventBus.Abstractions
{
    [SuppressMessage("Naming", "CA1711", Justification = "<Suspended>")]
    public interface IDynamicIntegrationEventHandler
    {
        Task HandleAsync(dynamic eventData);
    }
}
