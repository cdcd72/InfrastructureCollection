#pragma warning disable CA1711

namespace Infra.Core.Background.Abstractions;

public interface IBackgroundTaskQueue
{
    string Name { get; }

    int QueuedCount { get; }

    ValueTask EnqueueAsync(Func<CancellationToken, ValueTask> workItem);

    ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken);
}
