using System.Threading.Channels;
using Infra.Core.Background.Abstractions;
using Infra.Core.Background.Models;

#pragma warning disable CA1711

namespace Infra.Core.Background;

public class BackgroundTaskQueue(BackgroundTaskQueueOptions backgroundTaskQueueOptions) : IBackgroundTaskQueue
{
    private readonly Channel<Func<CancellationToken, ValueTask>> queue =
        Channel.CreateBounded<Func<CancellationToken, ValueTask>>(
            new BoundedChannelOptions(backgroundTaskQueueOptions.Capacity)
            {
                FullMode = backgroundTaskQueueOptions.FullMode
            });

    #region Properties

    public string Name { get; } = backgroundTaskQueueOptions.Name;

    public int QueuedCount => queue.Reader.Count;

    #endregion

    public async ValueTask EnqueueAsync(Func<CancellationToken, ValueTask> workItem)
    {
        ArgumentNullException.ThrowIfNull(workItem);

        await queue.Writer.WriteAsync(workItem);
    }

    public async ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken) => await queue.Reader.ReadAsync(cancellationToken);
}
