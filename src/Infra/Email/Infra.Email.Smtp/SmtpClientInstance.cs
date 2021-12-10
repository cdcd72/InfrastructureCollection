using System;
using System.Linq;
using System.Threading.Tasks;
using Infra.Core.Email.Abstractions;
using Infra.Core.Email.Models;
using Infra.Core.Extensions;
using Infra.Email.Smtp.Configuration;
using Infra.Email.Smtp.Configuration.Validators;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Infra.Email.Smtp
{
    public class SmtpClientInstance : IMail
    {
        private readonly ILogger<SmtpClientInstance> _logger;
        private readonly Settings _settings;

        public SmtpClientInstance(
            ILogger<SmtpClientInstance> logger,
            IOptions<Settings> settings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = SettingsValidator.TryValidate(settings.Value, out var validationException) ? settings.Value : throw validationException;
        }

        #region Sync Method

        public bool Send(MailParam mailParam)
            => Task.Run(() => SendAsync(mailParam)).GetAwaiter().GetResult();

        #endregion

        #region Async Method

        public async Task<bool> SendAsync(MailParam mailParam)
        {
            var host = _settings.Host;

            // Check host isn't empty.
            if (string.IsNullOrWhiteSpace(host))
                return false;

            var port = _settings.Port;
            var account = _settings.Account;
            var password = _settings.Password;

            // Get mail message.
            var message = GetMimeMessage(mailParam);

            using var smtpClient = new SmtpClient();

            try
            {
                if (!string.IsNullOrWhiteSpace(account) && !string.IsNullOrWhiteSpace(password))
                {
                    await smtpClient.ConnectAsync(host, port, true);
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
                _logger.Error($"Send mail error: {ex.Message}");
                throw;
            }
            finally
            {
                await smtpClient.DisconnectAsync(true);
            }

            return true;
        }

        #endregion

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
                message.To.AddRange(mailParam.Mailto.Select(mailTo => MailboxAddress.Parse(mailTo)));

            if (mailParam.Cc?.Count > 0)
                // Add carbon copy receivers
                message.Cc.AddRange(mailParam.Cc.Select(ccTo => MailboxAddress.Parse(ccTo)));

            if (mailParam.Bcc?.Count > 0)
                // Add blind carbon copy receivers
                message.Bcc.AddRange(mailParam.Bcc.Select(bccTo => MailboxAddress.Parse(bccTo)));

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
                foreach (var attachmentKey in mailParam.Attachment.Keys)
                {
                    var attachmentName = attachmentKey;
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
