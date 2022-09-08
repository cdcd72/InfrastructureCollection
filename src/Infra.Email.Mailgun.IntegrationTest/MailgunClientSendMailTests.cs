using System.Reflection;
using Infra.Core.Email.Abstractions;
using Infra.Core.Email.Models;
using NUnit.Framework;

namespace Infra.Email.Mailgun.IntegrationTest
{
    public class MailgunClientSendMailTests
    {
        private readonly IMailClient _mailClient;

        private static string CurrentDirectory =>
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

        private static string SenderAddress => "unittest-dev@smtp.mailgun.org";

        private static string SenderName => "TestUser";

        private static List<string> Mailto => new() { "liangyun.stark@gmail.com" };

        private static List<string> Ccto => new() { "a6898208@gmail.com" };

        private static List<string> Bccto => new() { };

        private static string Message => $@"
    Environment.MachineName: {Environment.MachineName}
    Environment.OSVersion.VersionString: {Environment.OSVersion.VersionString}";

        public MailgunClientSendMailTests()
        {
            var startup = new Startup();

            _mailClient = startup.GetService<IMailClient>();
        }

        [Test]
        public async Task SendPlainTextMailSuccessAsync()
        {
            // Arrange
            var mailParam = new MailParam()
            {
                SenderAddress = SenderAddress,
                SenderName = SenderName,
                Mailto = Mailto,
                Subject = "Send plain text mail success.",
                Message = Message,
                IsHtml = false
            };

            // Act
            var result = await _mailClient.SendAsync(mailParam);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task SendHtmlMailSuccessAsync()
        {
            // Arrange
            var mailParam = new MailParam()
            {
                SenderAddress = SenderAddress,
                SenderName = SenderName,
                Mailto = Mailto,
                Subject = "Send html content mail success.",
                Message = $"<b>{Message}</b>",
                IsHtml = true
            };

            // Act
            var result = await _mailClient.SendAsync(mailParam);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task SendMailWithCcSuccessAsync()
        {
            // Arrange
            var mailParam = new MailParam()
            {
                SenderAddress = SenderAddress,
                SenderName = SenderName,
                Mailto = Mailto,
                Cc = Ccto,
                Subject = "Send mail with carbon copy success.",
                Message = Message,
                IsHtml = false
            };

            // Act
            var result = await _mailClient.SendAsync(mailParam);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task SendMailWithBccSuccessAsync()
        {
            // Arrange
            var mailParam = new MailParam()
            {
                SenderAddress = SenderAddress,
                SenderName = SenderName,
                Mailto = Mailto,
                Bcc = Bccto,
                Subject = "Send mail with blind carbon copy success.",
                Message = Message,
                IsHtml = false
            };

            // Act
            var result = await _mailClient.SendAsync(mailParam);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task SendMailWithAttachmentSuccessAsync()
        {
            // Arrange
            var filePath = Path.Combine(CurrentDirectory, "TestData", "Files", "test.jpg");
            var fileName = Path.GetFileName(filePath);
            var fileBytes = await File.ReadAllBytesAsync(filePath);

            var attachmentDic = new Dictionary<string, byte[]>()
            {
                { fileName, fileBytes }
            };

            var mailParam = new MailParam()
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
            var result = await _mailClient.SendAsync(mailParam);

            // Assert
            Assert.IsTrue(result);
        }
    }
}
