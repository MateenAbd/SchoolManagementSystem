using System.Threading;
using System.Threading.Tasks;

namespace SMS.Core.Interfaces
{
    public interface ISmsSender
    {
        Task<bool> SendSmsAsync(string toPhone, string message, CancellationToken token = default);
    }
}