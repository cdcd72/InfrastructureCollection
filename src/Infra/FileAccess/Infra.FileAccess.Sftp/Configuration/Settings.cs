namespace Infra.FileAccess.Sftp.Configuration;

public class Settings
{
    public const string SectionName = "File:Sftp";

    /// <summary>
    /// SFTP Host
    /// </summary>
    public string Host { get; set; }

    /// <summary>
    /// SFTP Port
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// User
    /// </summary>
    public string User { get; set; }

    /// <summary>
    /// Password
    /// </summary>
    public string Password { get; set; }
}