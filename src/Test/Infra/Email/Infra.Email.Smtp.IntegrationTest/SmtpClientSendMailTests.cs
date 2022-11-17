using System.Reflection;
using Infra.Core.Email.Abstractions;
using Infra.Core.Email.Models;
using NUnit.Framework;

namespace Infra.Email.Smtp.IntegrationTest
{
    /// <summary>
    /// Smtp client send mail integration test cases
    /// !!!
    /// !!! Notice: Private smtp server info, please don't commit into version control.
    /// !!!         There integration test cases for convenient test use.
    /// !!!
    /// </summary>
    public class SmtpClientSendMailTests
    {
        private readonly IMailClient mailClient;

        #region Properties

        private static string CurrentDirectory =>
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

        private static string SenderAddress => string.Empty;

        private static string SenderName => "TestUser";

        private static List<string> Mailto => new();

        private static List<string> CcTo => new();

        private static List<string> BccTo => new();

        private static string Message => $@"
    Environment.MachineName: {Environment.MachineName}
    Environment.OSVersion.VersionString: {Environment.OSVersion.VersionString}";

        #endregion

        public SmtpClientSendMailTests()
        {
            var startup = new Startup();

            mailClient = startup.GetService<IMailClient>();
        }

        [Test]
        public void SendPlainTextMailSuccess()
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
            var task = mailClient.SendAsync(mailParam);

            // Assert
            Assert.That(task.Status, Is.EqualTo(TaskStatus.RanToCompletion));
        }

        [Test]
        public void SendHtmlMailSuccess()
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
            var task = mailClient.SendAsync(mailParam);

            // Assert
            Assert.That(task.Status, Is.EqualTo(TaskStatus.RanToCompletion));
        }

        [Test]
        public void SendMailWithCcSuccess()
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
            var task = mailClient.SendAsync(mailParam);

            // Assert
            Assert.That(task.Status, Is.EqualTo(TaskStatus.RanToCompletion));
        }

        [Test]
        public void SendMailWithBccSuccess()
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
            var task = mailClient.SendAsync(mailParam);

            // Assert
            Assert.That(task.Status, Is.EqualTo(TaskStatus.RanToCompletion));
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
            var task = mailClient.SendAsync(mailParam);

            // Assert
            Assert.That(task.Status, Is.EqualTo(TaskStatus.RanToCompletion));
        }
    }
}
