namespace GrpcFileServer.Configuration
{
    public class Settings
    {
        public const string SectionName = "Grpc:File";

        /// <summary>
        /// Files saved directory
        /// </summary>
        public string Root { get; set; }

        /// <summary>
        /// File chunk size
        /// </summary>
        public int ChunkSize { get; set; }

        /// <summary>
        /// File chunk buffer
        /// </summary>
        public int ChunkBufferCount { get; set; }
    }
}
