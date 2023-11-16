using Infra.Core.Time.Abstractions;
using Infra.Time.Configuration;
using Infra.Time.Configuration.Validators;
using Microsoft.Extensions.Options;

namespace Infra.Time;

public class TimeSpanHelper : ITimeSpanHelper
{
    private readonly Settings settings;

    public TimeSpanHelper(IOptions<Settings> settings) => this.settings = SettingsValidator.TryValidate(settings.Value, out var validationException) ? settings.Value : throw validationException;

    public TimeSpan GetExpressionMatchTimeout(TimeSpan? timeout = null) => timeout ?? TimeSpan.FromMilliseconds(settings.ExpressionMatchTimeout);
}
