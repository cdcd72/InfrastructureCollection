using System.Threading.Tasks;
using Infra.Core.Email.Models;

namespace Infra.Core.Email.Abstractions
{
    public interface IMail
    {
        /// <summary>
        /// Send mail
        /// </summary>
        /// <param name="mailParam">Mail parameter</param>
        /// <param name="otherParam">Other parameter</param>
        /// <returns></returns>
        bool Send(MailParam mailParam, object otherParam = null);

        /// <summary>
        /// Send mail
        /// </summary>
        /// <param name="mailParam">Mail parameter</param>
        /// <param name="otherParam">Other parameter</param>
        /// <returns></returns>
        Task<bool> SendAsync(MailParam mailParam, object otherParam = null);
    }
}
