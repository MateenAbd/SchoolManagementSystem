using System.Threading;
using System.Threading.Tasks;

namespace SMS.Core.Interfaces
{
    public interface IEmailSender
    {
        Task<bool> SendEmailAsync(string to, string subject, string body, CancellationToken token = default);
    }
}