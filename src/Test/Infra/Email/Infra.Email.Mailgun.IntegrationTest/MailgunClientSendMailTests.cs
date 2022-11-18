using System.Reflection;
using Infra.Core.Email.Abstractions;
using Infra.Core.Email.Models;
using NUnit.Framework;

namespace Infra.Email.Mailgun.IntegrationTest
{
    public class MailgunClientSendMailTests
    {
        private readonly IMailClient mailClient;

        private static string CurrentDirectory =>
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

        private static string SenderAddress => "testuser@smtp.mailgun.org";

        private static string SenderName => "TestUser";

        private static List<string> Mailto => new();

        private static List<string> CcTo => new();

        private static List<string> BccTo => new();

        private static string Message => $@"
    Environment.MachineName: {Environment.MachineName}
    Environment.OSVersion.VersionString: {Environment.OSVersion.VersionString}";

        public MailgunClientSendMailTests()
        {
            var startup = new Startup();

            mailClient = startup.GetService<IMailClient>();
        }

        [Test]
        public async Task SendPlainTextMailSuccess()
        {
            // Arrange
            var mailParam = new MailParam
            {
                SenderAddress = SenderAddress,
                SenderName = SenderName,
                Mailto = Mailto,
                Subject = "Send plain text mail success.",
                Message = Message,
                IsHtml = false
            };

            // Act
            await mailClient.SendAsync(mailParam);
        }

        [Test]
        public async Task SendHtmlMailSuccess()
        {
            // Arrange
            var mailParam = new MailParam
            {
                SenderAddress = SenderAddress,
                SenderName = SenderName,
                Mailto = Mailto,
                Subject = "Send html content mail success.",
                Message = $"<b>{Message}</b>",
                IsHtml = true
            };

            // Act
            await mailClient.SendAsync(mailParam);
        }

        [Test]
        public async Task SendMailWithCcSuccess()
        {
            // Arrange
            var mailParam = new MailParam
            {
                SenderAddress = SenderAddress,
                SenderName = SenderName,
                Mailto = Mailto,
                Cc = CcTo,
                Subject = "Send mail with carbon copy success.",
                Message = Message,
                IsHtml = false
            };

            // Act
            await mailClient.SendAsync(mailParam);
        }

        [Test]
        public async Task SendMailWithBccSuccess()
        {
            // Arrange
            var mailParam = new MailParam
            {
                SenderAddress = SenderAddress,
                SenderName = SenderName,
                Mailto = Mailto,
                Bcc = BccTo,
                Subject = "Send mail with blind carbon copy success.",
                Message = Message,
                IsHtml = false
            };

            // Act
            await mailClient.SendAsync(mailParam);
        }

        [Test]
        public async Task SendMailWithAttachmentSuccess()
        {
            // Arrange
            var filePath = Path.Combine(CurrentDirectory, "TestData", "Files", "test.jpg");
            var fileName = Path.GetFileName(filePath);
            var fileBytes = await File.ReadAllBytesAsync(filePath);

            var attachmentDic = new Dictionary<string, byte[]>
            {
                { fileName, fileBytes }
            };

            var mailParam = new MailParam
            {
                SenderAddress = SenderAddress,
                SenderName = SenderName,
                Mailto = Mailto,
                Subject = "Send mail with attachment success.",
                Message = Message,
                Attachment = attachmentDic,
                IsHtml = false
            };

            // Act
            await mailClient.SendAsync(mailParam);
        }
    }
}
