#pragma warning disable CA2208

namespace Infra.FileAccess.Physical.Configuration.Validators;

public static class SettingsValidator
{
    public static bool TryValidate(Settings settings, out AggregateException validationExceptions)
    {
        ArgumentNullException.ThrowIfNull(settings);

        var exceptions = new List<Exception>();

        if (settings.Roots is null || (settings.Roots is not null && settings.Roots.Any(root => root is "")))
            exceptions.Add(new ArgumentNullException(nameof(settings.Roots)));

        validationExceptions = new AggregateException(exceptions);

        return exceptions.Count == 0;
    }
}
