using Infra.Core.Background.Abstractions;
using Infra.Core.Extensions;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Infra.Core.IntegrationTest.Background;

public class BackgroundTaskQueueTests
{
    private readonly ILogger<BackgroundTaskQueueTests> logger;
    private readonly IBackgroundTaskQueue queue;

    public BackgroundTaskQueueTests()
    {
        var startup = new Startup();

        logger = startup.GetService<ILogger<BackgroundTaskQueueTests>>();
        queue = startup.GetService<IEnumerable<IBackgroundTaskQueue>>().GetBackgroundTaskQueue("Demo");
    }

    [Test]
    public void EnqueueFail() => Assert.ThrowsAsync<ArgumentNullException>(async () => await queue.EnqueueAsync(null));

    [Test]
    public async Task EnqueueSuccess()
    {
        await queue.EnqueueAsync(token => BuildCustomWorkItem("value", token));

        Assert.That(queue.QueuedCount, Is.EqualTo(1));
    }

    [Test]
    public async Task DequeueSuccess()
    {
        await EnqueueSuccess();

        var stoppingToken = new CancellationToken();

        var workItem = await queue.DequeueAsync(stoppingToken);

        await workItem(stoppingToken);

        Assert.That(queue.QueuedCount, Is.EqualTo(0));
    }

    [TearDown]
    public async Task TearDown()
    {
        if (queue.QueuedCount > 0)
        {
            for (var i = 0; i < queue.QueuedCount; i++)
            {
                await queue.DequeueAsync(new CancellationToken());
            }
        }
    }

    #region Private Method

    private async ValueTask BuildCustomWorkItem(string value, CancellationToken token)
    {
        // Simulate one 3-second task to complete
        // for each enqueued work item

        token.ThrowIfCancellationRequested();

        var guid = $"{Guid.NewGuid()}";

        logger.Information($"Queued Background Task {guid} is starting.");

        try
        {
            logger.Information($"Queued Background Task {guid} is running with value {value}.");

            await Task.Delay(TimeSpan.FromSeconds(3), token);
        }
        catch (OperationCanceledException)
        {
            // Prevent throwing if the Delay is cancelled
        }

        logger.Information($"Queued Background Task {guid} is complete.");
    }

    #endregion
}
