using Microsoft.Extensions.Configuration;

namespace Infra.FileAccess.Grpc.Common
{
    public class Env
    {
        private const string SectionName = "Grpc:File";

        public string ServerAddress { get; set; }

        public int ChunkSize { get; set; }

        public int ChunkBufferCount { get; set; }

        #region Constructor

        public Env(IConfiguration config) => InitEnv(config);

        #endregion

        #region Private Method

        private void InitEnv(IConfiguration config)
        {
            ServerAddress = config.GetValue<string>($"{SectionName}:{nameof(ServerAddress)}");
            ChunkSize = config.GetValue<int>($"{SectionName}:{nameof(ChunkSize)}");
            ChunkBufferCount = config.GetValue<int>($"{SectionName}:{nameof(ChunkBufferCount)}");
        }

        #endregion
    }
}
