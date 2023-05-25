using Microsoft.Extensions.DependencyInjection;
using System;

namespace EmailLibrary
{
    public static class EmailServiceExtensions
    {
        public static void AddEmail(this IServiceCollection services, Action<EmailSettings> configureOptions)
        {
            services.Configure<EmailSettings>(configureOptions);
            services.AddTransient<IMailService, MailService>();
        }
    }
}
