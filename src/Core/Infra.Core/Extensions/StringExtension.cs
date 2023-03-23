using System.Security;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Web;
using System.Xml;

namespace Infra.Core.Extensions;

public static class StringExtension
{
    private static string[] DefaultExcludeStringsForLogForging => new[] { "%0a", "%0d", "%0A", "%0D", "\r", "\n" };

    public static bool ToBoolean(this string value) => bool.TryParse(value, out var result) && result;

    public static int ToInt(this string value) => int.TryParse(value, out var result) ? result : 0;

    public static long ToLong(this string value) => long.TryParse(value, out var result) ? result : 0;

    public static float ToFloat(this string value) => float.TryParse(value, out var result) ? result : 0f;

    public static double ToDouble(this string value) => double.TryParse(value, out var result) ? result : 0.0;

    public static SecureString ToSecureString(this string rawString)
    {
        var secureString = new SecureString();

        foreach (var rawChar in rawString)
        {
            secureString.AppendChar(rawChar);
        }

        secureString.MakeReadOnly();

        return secureString;
    }

    public static XmlReader GetXmlReader(this string xmlContent)
    {
        var xmlStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlContent));

        var settings = new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null
        };

        return XmlReader.Create(xmlStream, settings);
    }

    public static string PreventLogForging(this string content, string[] excludeStrings = null)
    {
        content ??= string.Empty;

        excludeStrings ??= DefaultExcludeStringsForLogForging;

        var normalizedLogContent = content.Normalize(NormalizationForm.FormKC);

        return excludeStrings.Aggregate(normalizedLogContent, (current, excludeString) => current.Replace(excludeString, string.Empty));
    }

    public static string HtmlEncode(this string value) => HttpUtility.HtmlEncode(value);

    public static string HtmlDecode(this string value) => HttpUtility.HtmlDecode(value);

    public static string UrlEncode(this string value) => HttpUtility.UrlEncode(value);

    public static string UrlDecode(this string value) => HttpUtility.UrlDecode(value);

    public static T FromJson<T>(this string json, TextEncoderSettings encoderSettings = null)
    {
        if (encoderSettings is null)
        {
            encoderSettings = new TextEncoderSettings();

            // https://github.com/dotnet/runtime/issues/2374
            encoderSettings.AllowRange(UnicodeRanges.All);
        }

        return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(encoderSettings)
        });
    }

    public static string AddNewLine(this string value) => $"{value}{Environment.NewLine}";
}
