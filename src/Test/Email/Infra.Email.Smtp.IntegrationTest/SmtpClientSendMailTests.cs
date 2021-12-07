using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Infra.Core.Email.Models;
using Infra.Email.Smtp.Exceptions;
using Infra.Email.Smtp.Models;
using Microsoft.Extensions.Logging;
using Moq;
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
        #region Properties

        private static string CurrentDirectory =>
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

        private static string Host => string.Empty;

        private static string Account => string.Empty;

        private static string Password => string.Empty;

        private static string SenderAddress => string.Empty;

        private static string SenderName => "TestUser";

        private static List<string> Mailto => new() { };

        private static List<string> Ccto => new() { };

        private static List<string> Bccto => new() { };

        private static string Message => $@"
    Environment.MachineName: {Environment.MachineName}
    Environment.OSVersion.VersionString: {Environment.OSVersion.VersionString}";

        #endregion

        [Test]
        public void SendMailFailWithCastSmtpParamFailedAsync()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<SmtpClientInstance>>();

            var smtpClientSendMail = new SmtpClientInstance(mockLogger.Object);

            // Simulate smtpParam object type isn't Infra.Email.Smtp.Models.SmtpParam.
            var smtpParam = new { };

            var mailParam = new MailParam()
            {
                SenderAddress = SenderAddress,
                SenderName = SenderName,
                Mailto = Mailto,
                Subject = "Send mail fail with cast smtp parameter failed.",
                Message = Message,
                IsHtml = false
            };

            // Act & Assert
            Assert.ThrowsAsync<CastSmtpParamFailException>(() => smtpClientSendMail.SendAsync(mailParam, smtpParam));
        }

        [Test]
        public async Task SendMailFailWithNoneHostAsync()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<SmtpClientInstance>>();

            var smtpClientSendMail = new SmtpClientInstance(mockLogger.Object);

            // Simulate none host
            var smtpParam = new SmtpParam()
            {
                Host = string.Empty
            };

            var mailParam = new MailParam()
            {
                SenderAddress = SenderAddress,
                SenderName = SenderName,
                Mailto = Mailto,
                Subject = "Send mail fail with none host.",
                Message = Message,
                IsHtml = false
            };

            // Act
            var result = await smtpClientSendMail.SendAsync(mailParam, smtpParam);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void SendMailFailWithUnexpectedArgumentOutOfRangeExceptionAsync()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<SmtpClientInstance>>();

            var smtpClientSendMail = new SmtpClientInstance(mockLogger.Object);

            // Simulate unexpected port number
            var smtpParam = new SmtpParam()
            {
                Host = Host,
                Port = 999999999
            };

            var mailParam = new MailParam()
            {
                SenderAddress = SenderAddress,
                SenderName = SenderName,
                Mailto = Mailto,
                Subject = "Send mail fail with unexpected argument out of range exception.",
                Message = Message,
                IsHtml = false
            };

            // Act & Assert
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => smtpClientSendMail.SendAsync(mailParam, smtpParam));

            mockLogger.Verify(
                m => m.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((message, _) => $"{message}".Contains("Send mail error:")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task SendPlainTextMailSuccessAsync()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<SmtpClientInstance>>();

            var smtpClientSendMail = new SmtpClientInstance(mockLogger.Object);

            var smtpParam = new SmtpParam()
            {
                Host = Host
            };

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
            var result = await smtpClientSendMail.SendAsync(mailParam, smtpParam);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task SendHtmlMailSuccessAsync()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<SmtpClientInstance>>();

            var smtpClientSendMail = new SmtpClientInstance(mockLogger.Object);

            var smtpParam = new SmtpParam()
            {
                Host = Host
            };

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
            var result = await smtpClientSendMail.SendAsync(mailParam, smtpParam);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task SendMailWithCcSuccessAsync()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<SmtpClientInstance>>();

            var smtpClientSendMail = new SmtpClientInstance(mockLogger.Object);

            var smtpParam = new SmtpParam()
            {
                Host = Host
            };

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
            var result = await smtpClientSendMail.SendAsync(mailParam, smtpParam);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task SendMailWithBccSuccessAsync()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<SmtpClientInstance>>();

            var smtpClientSendMail = new SmtpClientInstance(mockLogger.Object);

            var smtpParam = new SmtpParam()
            {
                Host = Host
            };

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
            var result = await smtpClientSendMail.SendAsync(mailParam, smtpParam);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task SendMailWithAttachmentSuccessAsync()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<SmtpClientInstance>>();

            var smtpClientSendMail = new SmtpClientInstance(mockLogger.Object);

            var smtpParam = new SmtpParam()
            {
                Host = Host
            };

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
            var result = await smtpClientSendMail.SendAsync(mailParam, smtpParam);

            // Assert
            Assert.IsTrue(result);
        }
    }
}
