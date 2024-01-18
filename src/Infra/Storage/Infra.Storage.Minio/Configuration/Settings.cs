namespace Infra.Storage.Minio.Configuration;

public class Settings
{
    public const string SectionName = "Storage:Minio";

    /// <summary>
    /// Url to object storage service.
    /// </summary>
    public string Endpoint { get; set; }

    /// <summary>
    /// Access key is the user ID that uniquely identifies your account.
    /// </summary>
    public string AccessKey { get; set; }

    /// <summary>
    /// Secret key is the password to your account.
    /// </summary>
    public string SecretKey { get; set; }

    /// <summary>
    /// Set timeout for all requests. (Timeout in milliseconds)
    /// </summary>
    public int Timeout { get; set; }

    /// <summary>
    /// Connects to object storage service with https.
    /// </summary>
    public bool EnableSsl { get; set; }
}