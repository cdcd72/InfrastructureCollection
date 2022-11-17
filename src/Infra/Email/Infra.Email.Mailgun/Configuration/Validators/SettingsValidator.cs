#pragma warning disable CA2208

namespace Infra.Email.Mailgun.Configuration.Validators
{
    public static class SettingsValidator
    {
        public static bool TryValidate(Settings settings, out AggregateException validationExceptions)
        {
            if (settings is null) throw new ArgumentNullException(nameof(settings));

            var exceptions = new List<Exception>();

            if (string.IsNullOrWhiteSpace(settings.ApiBaseUrl))
                exceptions.Add(new ArgumentNullException(nameof(settings.ApiBaseUrl)));

            if (string.IsNullOrWhiteSpace(settings.ApiKey))
                exceptions.Add(new ArgumentNullException(nameof(settings.ApiKey)));

            validationExceptions = new AggregateException(exceptions);

            return !exceptions.Any();
        }
    }
}
