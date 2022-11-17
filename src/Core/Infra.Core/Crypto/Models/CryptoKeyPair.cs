namespace Infra.Core.Crypto.Models;

public class CryptoKeyPair
{
    public byte[] Key { get; init; }

    public byte[] Iv { get; init; }

    public static CryptoKeyPair Parse(string base64Key, string base64Iv) =>
        new()
        {
            Key = Convert.FromBase64String(base64Key),
            Iv = Convert.FromBase64String(base64Iv)
        };
}
