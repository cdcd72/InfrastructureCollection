namespace Infra.Core.Configuration;

public class FileValidateSettings
{
    public const string SectionName = "File:Validate";

    public string[] AllowedFileExtensions { get; set; }

    public int AllowedFileNameLength { get; set; }

    public long AllowedFileSize { get; set; }

    public string[] NotAllowedFileNameSpecialSymbols { get; set; }
}
