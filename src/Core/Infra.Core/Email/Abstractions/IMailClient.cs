using Infra.Core.Email.Models;

namespace Infra.Core.Email.Abstractions;

public interface IMailClient
{
    Task SendAsync(MailParam mailParam);
}