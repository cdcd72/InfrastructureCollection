namespace Infra.FileAccess.Grpc.Configuration;

public class Settings
{
    public const string SectionName = "File:Grpc:Client";

    /// <summary>
    /// gRPC server address
    /// </summary>
    public string ServerAddress { get; set; }

    /// <summary>
    /// File chunk size
    /// </summary>
    public int ChunkSize { get; set; }

    /// <summary>
    /// File chunk buffer
    /// </summary>
    public int ChunkBufferCount { get; set; }
}