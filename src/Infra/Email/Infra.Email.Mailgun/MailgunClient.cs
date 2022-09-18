using System.Net.Http.Headers;
using System.Text;
using Infra.Core.Email.Abstractions;
using Infra.Core.Email.Models;
using Infra.Core.Extensions;
using Infra.Email.Mailgun.Configuration;
using Infra.Email.Mailgun.Configuration.Validators;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infra.Email.Mailgun
{
    public class MailgunClient : IMailClient
    {
        private readonly ILogger<MailgunClient> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly Settings _settings;

        public MailgunClient(
            ILogger<MailgunClient> logger,
            IHttpClientFactory httpClientFactory,
            IOptions<Settings> settings
        )
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _settings = SettingsValidator.TryValidate(settings.Value, out var validationException) ? settings.Value : throw validationException;
        }

        public async Task<bool> SendAsync(MailParam mailParam)
        {
            var apiBaseUrl = _settings.ApiBaseUrl.EndsWith("/") ? _settings.ApiBaseUrl : $"{_settings.ApiBaseUrl}/";

            try
            {
                var client = _httpClientFactory.CreateClient();

                client.BaseAddress = new Uri(apiBaseUrl);

                var byteArray = Encoding.UTF8.GetBytes($"api:{_settings.ApiKey}");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var content = GetMailContent(mailParam);

                await client.PostAsync("messages", content);
            }
            catch (Exception ex)
            {
                _logger.Error($"Send mail error: {ex.Message}");
                throw;
            }

            return true;
        }

        private static MultipartFormDataContent GetMailContent(MailParam mailParams)
        {
            var content = new MultipartFormDataContent
            {
                { new StringContent(mailParams.SenderAddress), "from" },
                { new StringContent(mailParams.Subject), "subject" }
            };

            // Set body content
            if (mailParams.IsHtml)
            {
                content.Add(new StringContent(mailParams.Message), "html");
            }
            else
            {
                content.Add(new StringContent(mailParams.Message), "text");
            }

            // Set receivers
            if (mailParams.Mailto?.Count > 0)
            {
                foreach (var to in mailParams.Mailto)
                {
                    content.Add(new StringContent(to), "to");
                }
            }

            // Set cc
            if (mailParams.Cc?.Count > 0)
            {
                foreach (var cc in mailParams.Cc)
                {
                    content.Add(new StringContent(cc), "cc");
                }
            }

            // Set bcc
            if (mailParams.Bcc?.Count > 0)
            {
                foreach (var bcc in mailParams.Bcc)
                {
                    content.Add(new StringContent(bcc), "bcc");
                }
            }

            // Set attachment
            if (mailParams.Attachment?.Count > 0)
            {
                foreach (var attachmentKey in mailParams.Attachment.Keys)
                {
                    var attachmentName = attachmentKey;
                    var attachmentBytes = mailParams.Attachment[attachmentName];

                    var fileContent = new ByteArrayContent(attachmentBytes);

                    content.Add(fileContent, "attachment", attachmentName);
                }
            }

            return content;
        }
    }
}
