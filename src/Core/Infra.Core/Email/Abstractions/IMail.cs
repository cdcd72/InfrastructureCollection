using System.Threading.Tasks;
using Infra.Core.Email.Models;

namespace Infra.Core.Email.Abstractions
{
    public interface IMail
    {
        #region Sync Method

        bool Send(MailParam mailParam, object otherParam = null);

        #endregion

        #region Async Method

        Task<bool> SendAsync(MailParam mailParam, object otherParam = null);

        #endregion
    }
}
