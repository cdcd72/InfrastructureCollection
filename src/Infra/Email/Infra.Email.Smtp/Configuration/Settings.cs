namespace Infra.Email.Smtp.Configuration
{
    public class Settings
    {
        public const string SectionName = "Email";

        /// <summary>
        /// Smtp Host
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Smtp Port
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Account
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }
    }
}
