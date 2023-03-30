using Infra.Core.Background.Abstractions;

namespace Infra.Core.Extensions;

public static class BackgroundTaskQueueExtension
{
    public static IBackgroundTaskQueue GetBackgroundTaskQueue(this IEnumerable<IBackgroundTaskQueue> queues, string name) => queues.First(queue => queue.Name == name);
}
