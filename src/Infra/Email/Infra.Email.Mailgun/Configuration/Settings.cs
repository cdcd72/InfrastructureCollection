namespace Infra.Email.Mailgun.Configuration
{
    public class Settings
    {
        public const string SectionName = "Email:Mailgun";

        /// <summary>
        /// Mailgun API base URL
        /// </summary>
        public string ApiBaseUrl { get; set; }

        /// <summary>
        /// Mailgun API key
        /// </summary>
        public string ApiKey { get; set; }
    }
}
