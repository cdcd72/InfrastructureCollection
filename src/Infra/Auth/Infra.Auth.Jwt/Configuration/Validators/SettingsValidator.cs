#pragma warning disable CA2208

namespace Infra.Auth.Jwt.Configuration.Validators;

public static class SettingsValidator
{
    public static bool TryValidate(Settings settings, out AggregateException validationExceptions)
    {
        ArgumentNullException.ThrowIfNull(settings);

        var exceptions = new List<Exception>();

        if (string.IsNullOrWhiteSpace(settings.AccessTokenSecret))
            exceptions.Add(new ArgumentNullException(nameof(settings.AccessTokenSecret)));

        if (settings.AccessTokenExpirationMinutes < 0)
            exceptions.Add(new ArgumentOutOfRangeException(nameof(settings.AccessTokenExpirationMinutes)));

        if (string.IsNullOrWhiteSpace(settings.RefreshTokenSecret))
            exceptions.Add(new ArgumentNullException(nameof(settings.RefreshTokenSecret)));

        if (settings.RefreshTokenExpirationMinutes < 0)
            exceptions.Add(new ArgumentOutOfRangeException(nameof(settings.RefreshTokenExpirationMinutes)));

        if (string.IsNullOrWhiteSpace(settings.Issuer))
            exceptions.Add(new ArgumentNullException(nameof(settings.Issuer)));

        if (string.IsNullOrWhiteSpace(settings.Audience))
            exceptions.Add(new ArgumentNullException(nameof(settings.Audience)));

        validationExceptions = new AggregateException(exceptions);

        return exceptions.Count == 0;
    }
}
