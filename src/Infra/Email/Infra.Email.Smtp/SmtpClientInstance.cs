using System;
using System.Linq;
using System.Threading.Tasks;
using Infra.Core.Interfaces.Email;
using Infra.Core.Models.Email;
using Infra.Email.Smtp.Exceptions;
using Infra.Email.Smtp.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Infra.Email.Smtp
{
    public class SmtpClientInstance : IMail
    {
        private readonly ILogger<SmtpClientInstance> _logger;

        public SmtpClientInstance(ILogger<SmtpClientInstance> logger) => _logger = logger;

        #region Sync Method

        public bool Send(MailParam mailParam, object otherParam = null)
            => Task.Run(() => SendAsync(mailParam, otherParam)).GetAwaiter().GetResult();

        #endregion

        #region Async Method

        public async Task<bool> SendAsync(MailParam mailParam, object otherParam = null)
        {
            var smtpParam = GetSmtpParam(otherParam);
            var host = smtpParam.Host;

            // Check host isn't empty.
            if (string.IsNullOrWhiteSpace(host))
                return false;

            var port = smtpParam.Port;
            var account = smtpParam.Account;
            var password = smtpParam.Password;

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
                _logger.LogError($"Send mail error: {ex.Message}");
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
        /// Get SMTP（Simple Mail Transfer Protocol） parameter
        /// </summary>
        /// <param name="otherParam">Other parameter</param>
        /// <returns></returns>
        private static SmtpParam GetSmtpParam(object otherParam)
        {
            SmtpParam smtpParam;

            if (otherParam is SmtpParam)
                smtpParam = otherParam as SmtpParam;
            else
                throw new CastSmtpParamFailException($"{nameof(otherParam)} type must be Infra.Email.Smtp.Models.SmtpParam.");

            return smtpParam;
        }

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
