namespace Infra.Storage.Minio.Configuration
{
    public class Settings
    {
        public const string SectionName = "Storage:Minio";

        public string Endpoint { get; set; }

        public string AccessKey { get; set; }

        public string SecretKey { get; set; }

        public int Timeout { get; set; }

        public bool WithSSL { get; set; }
    }
}
