using Microsoft.Extensions.DependencyInjection;
using SMS.Application.Interfaces;
using SMS.Core.Interfaces;
using SMS.Core.Logger.Interfaces;
using SMS.Core.Logger.Services;
using SMS.Core.Repositories;
using SMS.Infrastructure.Repositories;
using SMS.Infrastructure.Services;

namespace SMS.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructure(this IServiceCollection services)
        {
            services.AddTransient<ILog, LogService>();
            services.AddTransient<IRepository, Repository>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            services.AddSingleton<IPasswordHasher, PasswordHasherService>();
            services.AddSingleton<IEmailSender, SmtpEmailSender>();
            services.AddSingleton<ISmsSender, DummySmsSender>();

        }
    }
}