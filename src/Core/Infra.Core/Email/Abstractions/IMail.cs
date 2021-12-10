using System.Threading.Tasks;
using Infra.Core.Email.Models;

namespace Infra.Core.Email.Abstractions
{
    public interface IMail
    {
        #region Sync Method

        bool Send(MailParam mailParam);

        #endregion

        #region Async Method

        Task<bool> SendAsync(MailParam mailParam);

        #endregion
    }
}
