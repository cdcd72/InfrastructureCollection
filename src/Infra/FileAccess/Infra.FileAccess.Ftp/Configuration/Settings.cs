namespace Infra.FileAccess.Ftp.Configuration;

public class Settings
{
    public const string SectionName = "File:Ftp";

    /// <summary>
    /// FTP Host
    /// </summary>
    public string Host { get; set; }

    /// <summary>
    /// FTP Port
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