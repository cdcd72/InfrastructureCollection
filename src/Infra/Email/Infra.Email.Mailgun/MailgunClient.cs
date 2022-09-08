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
        private readonly Settings _settings;

        public MailgunClient(
            ILogger<MailgunClient> logger,
            IOptions<Settings> settings
        )
        {
            _logger = logger;
            _settings = SettingsValidator.TryValidate(settings.Value, out var validationException) ? settings.Value : throw validationException;
        }

        public async Task<bool> SendAsync(MailParam mailParam)
        {
            var apiBaseUrl = _settings.ApiBaseUrl;
            var apiKey = _settings.ApiKey;

            try
            {
                var client = new HttpClient
                {
                    BaseAddress = new Uri($"{apiBaseUrl}")
                };

                var byteArray = new UTF8Encoding().GetBytes($"api:{apiKey}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var content = GetRequestFormContent(mailParam);

                await client.PostAsync("messages", content);
            }
            catch (Exception ex)
            {
                _logger.Error($"Send mail error: {ex.Message}");
                throw;
            }

            return true;
        }

        private static FormUrlEncodedContent GetRequestFormContent(MailParam mailParams)
        {
            var content = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("from", mailParams.SenderAddress),
                new KeyValuePair<string, string>("subject", mailParams.Subject)
            };

            // add body content
            if (mailParams.IsHtml)
            {
                content.Add(new KeyValuePair<string, string>("html", mailParams.Message));
            }
            else
            {
                content.Add(new KeyValuePair<string, string>("text", mailParams.Message));
            }

            // add receivers
            if (mailParams.Mailto?.Count > 0)
            {
                foreach (var to in mailParams.Mailto)
                {
                    content.Add(new KeyValuePair<string, string>("to", to));
                }
            }

            // add cc
            if (mailParams.Cc?.Count > 0)
            {
                foreach (var cc in mailParams.Cc)
                {
                    content.Add(new KeyValuePair<string, string>("cc", cc));
                }
            }

            // add bcc
            if (mailParams.Bcc?.Count > 0)
            {
                foreach (var cc in mailParams.Bcc)
                {
                    content.Add(new KeyValuePair<string, string>("bcc", cc));
                }
            }

            var formContent = new FormUrlEncodedContent(content);

            return formContent;
        }
    }
}
