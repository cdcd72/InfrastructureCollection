#pragma warning disable CA2208

namespace Infra.EventBus.RabbitMQ.Configuration.Validators
{
    public static class SettingsValidator
    {
        public static bool TryValidate(Settings settings, out AggregateException validationExceptions)
        {
            if (settings is null) throw new ArgumentNullException(nameof(settings));

            var exceptions = new List<Exception>();

            if (string.IsNullOrWhiteSpace(settings.QueueName))
                exceptions.Add(new ArgumentNullException(nameof(settings.QueueName)));

            if (settings.RetryCount < 5)
                exceptions.Add(new ArgumentNullException(nameof(settings.RetryCount)));

            validationExceptions = new AggregateException(exceptions);

            return !exceptions.Any();
        }
    }
}
