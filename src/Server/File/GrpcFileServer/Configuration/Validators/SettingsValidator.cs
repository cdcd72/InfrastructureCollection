﻿#pragma warning disable CA2208

namespace GrpcFileServer.Configuration.Validators;

public static class SettingsValidator
{
    public static bool TryValidate(Settings settings, out AggregateException validationExceptions)
    {
        ArgumentNullException.ThrowIfNull(settings);

        var exceptions = new List<Exception>();

        if (string.IsNullOrWhiteSpace(settings.Root))
            exceptions.Add(new ArgumentNullException(nameof(settings.Root)));

        if (settings.ChunkSize is 0)
            exceptions.Add(new ArgumentNullException(nameof(settings.ChunkSize)));

        if (settings.ChunkBufferCount is 0)
            exceptions.Add(new ArgumentNullException(nameof(settings.ChunkBufferCount)));

        validationExceptions = new AggregateException(exceptions);

        return exceptions.Count == 0;
    }
}
