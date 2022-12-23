using System.Xml.Linq;

namespace Infra.Core.Extensions;

public static class XDocumentExtension
{
    public static XDocument SetTemplateHeader(this XDocument xDoc, string version = "1.0", string encoding = "utf-8")
    {
        xDoc.Declaration = new XDeclaration(version, encoding, null);

        return xDoc;
    }

    public static string ToStringWithDecl(this XDocument xDoc, SaveOptions options)
    {
        var xml = xDoc.ToString(options)
            .Replace("\n", string.Empty)
            .Replace("\r", string.Empty)
            .Replace("\t", string.Empty);

        return $"{xDoc.Declaration}{xml}";
    }
}
