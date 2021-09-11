using System.Collections.Generic;

namespace Infra.Core.Models.Email
{
    public class MailParam
    {
        /// <summary>
        /// Subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Message（Html／Plaintext）
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Sender address
        /// </summary>
        public string SenderAddress { get; set; }

        /// <summary>
        /// Sender name
        /// </summary>
        public string SenderName { get; set; }

        /// <summary>
        /// Receiver addresses
        /// </summary>
        public List<string> Mailto { get; set; }

        /// <summary>
        /// Carbon copy addresses
        /// </summary>
        public List<string> Cc { get; set; }

        /// <summary>
        /// Blind carbon copy addresses
        /// </summary>
        public List<string> Bcc { get; set; }

        /// <summary>
        /// Attachment
        /// </summary>
        public Dictionary<string, byte[]> Attachment { get; set; }

        /// <summary>
        /// Html（true）／Plaintext（false）
        /// </summary>
        public bool IsHtml { get; set; }
    }
}
