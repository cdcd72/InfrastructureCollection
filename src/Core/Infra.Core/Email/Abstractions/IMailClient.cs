using System.Threading.Tasks;
using Infra.Core.Email.Models;

namespace Infra.Core.Email.Abstractions
{
    public interface IMailClient
    {
        Task<bool> SendAsync(MailParam mailParam);
    }
}
