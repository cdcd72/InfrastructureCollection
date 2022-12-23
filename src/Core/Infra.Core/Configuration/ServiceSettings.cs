namespace Infra.Core.Configuration;

public class ServiceSettings
{
    public const string SectionName = nameof(ServiceSettings);

    public string Name { get; set; }

    public string Url { get; set; }

    public int? Timeout { get; set; }
}
