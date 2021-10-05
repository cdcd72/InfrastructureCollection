using Microsoft.Extensions.Configuration;

namespace GrpcFileServer.Common
{
    public class Env
    {
        private const string SectionName = "Grpc:File";

        public string DirectoryRoot { get; set; }

        public int ChunkSize { get; set; }

        public int ChunkBufferCount { get; set; }

        #region Constructor

        public Env(IConfiguration config) => InitEnv(config);

        #endregion

        #region Private Method

        private void InitEnv(IConfiguration config)
        {
            DirectoryRoot = config.GetValue<string>($"{SectionName}:{nameof(DirectoryRoot)}");
            ChunkSize = config.GetValue<int>($"{SectionName}:{nameof(ChunkSize)}");
            ChunkBufferCount = config.GetValue<int>($"{SectionName}:{nameof(ChunkBufferCount)}");
        }

        #endregion
    }
}
