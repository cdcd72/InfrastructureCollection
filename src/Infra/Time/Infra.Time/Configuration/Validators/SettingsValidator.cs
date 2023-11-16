#pragma warning disable CA2208

namespace Infra.Time.Configuration.Validators
{
    public static class SettingsValidator
    {
        public static bool TryValidate(Settings settings, out AggregateException validationExceptions)
        {
            if (settings is null) throw new ArgumentNullException(nameof(settings));

            var exceptions = new List<Exception>();

            if (settings.ExpressionMatchTimeout < 0)
                exceptions.Add(new ArgumentOutOfRangeException(nameof(settings.ExpressionMatchTimeout)));

            validationExceptions = new AggregateException(exceptions);

            return !exceptions.Any();
        }
    }
}
