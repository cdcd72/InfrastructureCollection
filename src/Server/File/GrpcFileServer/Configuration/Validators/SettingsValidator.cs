using System;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable CA2208

namespace GrpcFileServer.Configuration.Validators
{
    public static class SettingsValidator
    {
        public static bool TryValidate(Settings settings, out AggregateException validationExceptions)
        {
            if (settings is null) throw new ArgumentNullException(nameof(settings));

            var exceptions = new List<Exception>();

            if (string.IsNullOrWhiteSpace(settings.Root))
                exceptions.Add(new ArgumentNullException(nameof(settings.Root)));

            if (settings.ChunkSize is 0)
                exceptions.Add(new ArgumentNullException(nameof(settings.ChunkSize)));

            if (settings.ChunkBufferCount is 0)
                exceptions.Add(new ArgumentNullException(nameof(settings.ChunkBufferCount)));

            validationExceptions = new AggregateException(exceptions);

            return !exceptions.Any();
        }
    }
}
