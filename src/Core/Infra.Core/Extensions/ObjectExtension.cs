using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Infra.Core.Extensions;

public static class ObjectExtension
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new();

    public static string ToJson(this object obj, TextEncoderSettings encoderSettings = null, bool writeIndented = false)
    {
        if (encoderSettings is null)
        {
            encoderSettings = new TextEncoderSettings();

            // https://github.com/dotnet/runtime/issues/2374
            encoderSettings.AllowRange(UnicodeRanges.All);
        }

        JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(encoderSettings);
        JsonSerializerOptions.WriteIndented = writeIndented;

        return JsonSerializer.Serialize(obj, JsonSerializerOptions);
    }
}
