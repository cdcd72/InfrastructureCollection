#pragma warning disable CA2208

namespace Infra.Storage.Minio.Configuration.Validators
{
    public static class SettingsValidator
    {
        public static bool TryValidate(Settings settings, out AggregateException validationExceptions)
        {
            if (settings is null) throw new ArgumentNullException(nameof(settings));

            var exceptions = new List<Exception>();

            if (string.IsNullOrWhiteSpace(settings.Endpoint))
                exceptions.Add(new ArgumentNullException(nameof(settings.Endpoint)));

            if (string.IsNullOrWhiteSpace(settings.AccessKey))
                exceptions.Add(new ArgumentNullException(nameof(settings.AccessKey)));

            if (string.IsNullOrWhiteSpace(settings.SecretKey))
                exceptions.Add(new ArgumentNullException(nameof(settings.SecretKey)));

            validationExceptions = new AggregateException(exceptions);

            return !exceptions.Any();
        }
    }
}
