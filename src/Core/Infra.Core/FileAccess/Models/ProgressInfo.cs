namespace Infra.Core.FileAccess.Models;

public class ProgressInfo
{
    public bool IsCompleted { get; set; }

    public string Message { get; set; }

    public string Result { get; set; }
}