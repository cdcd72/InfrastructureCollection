using System.Security;
using System.Text;

namespace Infra.Core.Extensions
{
    public static class StringExtension
    {
        private static string[] DefaultExcludeStringsForLogForging =>
            new string[] { "%0a", "%0d", "%0A", "%0D", "\r", "\n" };

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

        public static string PreventLogForging(this string content, string[] excludeStrings = null)
        {
            content ??= string.Empty;

            excludeStrings ??= DefaultExcludeStringsForLogForging;

            var normalizedLogContent = content.Normalize(NormalizationForm.FormKC);

            return excludeStrings.Aggregate(normalizedLogContent, (current, excludeString) => current.Replace(excludeString, string.Empty));
        }
    }
}
