using System.Threading.Channels;

namespace Infra.Core.Background.Models;

public class BackgroundTaskQueueOptions
{
    public const string SectionName = "BackgroundTaskQueues";

    public string Name { get; set; } = "Default";

    public int Capacity { get; set; } = 100;

    public BoundedChannelFullMode FullMode { get; set; } = BoundedChannelFullMode.Wait;
}
