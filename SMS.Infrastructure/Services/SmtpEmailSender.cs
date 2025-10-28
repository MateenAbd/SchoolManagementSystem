using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SMS.Core.Interfaces;
using SMS.Core.Logger.Interfaces;
using SMS.Core.Logger.Services;

namespace SMS.Infrastructure.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        private readonly ILog _logger;

        public SmtpEmailSender(IConfiguration config)
        {
            _config = config; _logger = new LogService();
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body, CancellationToken token = default)
        {
            var host = _config["Smtp:Host"];
            var port = int.TryParse(_config["Smtp:Port"], out var p) ? p : 587;
            var enableSsl = bool.TryParse(_config["Smtp:EnableSsl"], out var ssl) ? ssl : true;
            var user = _config["Smtp:User"];
            var pass = _config["Smtp:Pass"];
            var from = _config["Smtp:From"] ?? user;

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(from))
            {
                _logger.Information("SMTP not configured; skipping email send.");
                return false;
            }

            using var client = new SmtpClient(host, port)
            {
                EnableSsl = enableSsl,
                Credentials = string.IsNullOrWhiteSpace(user) ? CredentialCache.DefaultNetworkCredentials : new NetworkCredential(user, pass)
            };
            using var msg = new MailMessage(from!, to, subject ?? "", body) { IsBodyHtml = false };
            await client.SendMailAsync(msg, token);
            return true;
        }
    }
}