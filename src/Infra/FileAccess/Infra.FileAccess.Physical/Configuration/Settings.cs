namespace Infra.FileAccess.Physical.Configuration;

public class Settings
{
    public const string SectionName = "File";

    /// <summary>
    /// Root directories
    /// </summary>
    public string[] Roots { get; set; }
}