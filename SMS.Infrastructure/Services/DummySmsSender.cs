using SMS.Core.Logger.Interfaces;
using SMS.Core.Logger.Services;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SMS.Core.Interfaces;

namespace SMS.Infrastructure.Services
{
    public class DummySmsSender : ISmsSender
    {
        private readonly IConfiguration _config;
        private readonly ILog _logger;

        public DummySmsSender(IConfiguration config)
        {
            _config = config;
            _logger = new LogService();
        }

        public Task<bool> SendSmsAsync(string toPhone, string message, CancellationToken token = default)
        {
            var enabled = bool.TryParse(_config["Sms:Enabled"], out var e) ? e : false;
            if (!enabled)
            {
                _logger.Information($"[SMS abhi ke liye nahi charahe] Would send to {toPhone}: {message}");
                return Task.FromResult(false);
            }

            // we have to plug provider call here, abhi it just logs success
            _logger.Information($"[SMS sent] To {toPhone}: {message}");
            return Task.FromResult(true);
        }
    }
}