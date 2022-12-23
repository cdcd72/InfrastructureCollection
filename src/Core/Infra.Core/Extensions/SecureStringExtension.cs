using System.Net;
using System.Security;

namespace Infra.Core.Extensions;

public static class SecureStringExtension
{
    public static string ToRawString(this SecureString secureString)
        => new NetworkCredential(string.Empty, secureString).Password;
}
