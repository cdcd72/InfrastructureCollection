using Infra.Core.Email.Abstractions;
using Infra.Core.Email.Models;
using Infra.Core.Extensions;
using Infra.Email.Smtp.Configuration;
using Infra.Email.Smtp.Configuration.Validators;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Infra.Email.Smtp
{
    public class SmtpClient : IMailClient
    {
        private readonly ILogger<SmtpClient> logger;
        private readonly Settings settings;

        public SmtpClient(
            ILogger<SmtpClient> logger,
            IOptions<Settings> settings)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.settings = SettingsValidator.TryValidate(settings.Value, out var validationException) ? settings.Value : throw validationException;
        }

        public async Task SendAsync(MailParam mailParam)
        {
            var host = settings.Host;
            var port = settings.Port;
            var account = settings.Account;
            var password = settings.Password;

            // Get mail message.
            var message = GetMimeMessage(mailParam);

            using var smtpClient = new MailKit.Net.Smtp.SmtpClient();

            try
            {
                if (!string.IsNullOrWhiteSpace(account) && !string.IsNullOrWhiteSpace(password))
                {
                    await smtpClient.ConnectAsync(host, port, settings.EnableSsl);
                    await smtpClient.AuthenticateAsync(account, password);
                }
                else
                {
                    await smtpClient.ConnectAsync(host, port);
                }

                await smtpClient.SendAsync(message);
            }
            catch (Exception ex)
            {
                logger.Error($"Send mail error: {ex.Message}");
                throw;
            }
            finally
            {
                await smtpClient.DisconnectAsync(true);
            }
        }

        #region Private Method

        /// <summary>
        /// Get mail message
        /// </summary>
        /// <param name="mailParam">Mail parameter</param>
        /// <returns></returns>
        private static MimeMessage GetMimeMessage(MailParam mailParam)
        {
            var message = new MimeMessage();

            // Add sender
            message.From.Add(new MailboxAddress(mailParam.SenderName, mailParam.SenderAddress));

            if (mailParam.Mailto?.Count > 0)
                // Add receivers
                message.To.AddRange(mailParam.Mailto.Select(MailboxAddress.Parse));

            if (mailParam.Cc?.Count > 0)
                // Add carbon copy receivers
                message.Cc.AddRange(mailParam.Cc.Select(MailboxAddress.Parse));

            if (mailParam.Bcc?.Count > 0)
                // Add blind carbon copy receivers
                message.Bcc.AddRange(mailParam.Bcc.Select(MailboxAddress.Parse));

            // Set mail subject
            message.Subject = mailParam.Subject;

            // Build mail body
            var bodyBuilder = new BodyBuilder();

            if (mailParam.IsHtml)
                bodyBuilder.HtmlBody = mailParam.Message;
            else
                bodyBuilder.TextBody = mailParam.Message;

            if (mailParam.Attachment?.Count > 0)
            {
                foreach (var attachmentName in mailParam.Attachment.Keys)
                {
                    var attachmentBytes = mailParam.Attachment[attachmentName];

                    // Set attachment
                    bodyBuilder.Attachments.Add(attachmentName, attachmentBytes);
                }
            }

            // Set mail content
            message.Body = bodyBuilder.ToMessageBody();

            return message;
        }

        #endregion
    }
}
