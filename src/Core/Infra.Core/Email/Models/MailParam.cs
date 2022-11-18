namespace Infra.Core.Email.Models
{
    public class MailParam
    {
        /// <summary>
        /// Subject
        /// </summary>
        public string Subject { get; init; }

        /// <summary>
        /// Message（Html／Plaintext）
        /// </summary>
        public string Message { get; init; }

        /// <summary>
        /// Sender address
        /// </summary>
        public string SenderAddress { get; init; }

        /// <summary>
        /// Sender name
        /// </summary>
        public string SenderName { get; init; }

        /// <summary>
        /// Receiver addresses
        /// </summary>
        public List<string> Mailto { get; init; }

        /// <summary>
        /// Carbon copy addresses
        /// </summary>
        public List<string> Cc { get; init; }

        /// <summary>
        /// Blind carbon copy addresses
        /// </summary>
        public List<string> Bcc { get; init; }

        /// <summary>
        /// Attachment
        /// </summary>
        public Dictionary<string, byte[]> Attachment { get; init; }

        /// <summary>
        /// Html（true）／Plaintext（false）
        /// </summary>
        public bool IsHtml { get; init; }
    }
}
