using System.Threading.Tasks;

namespace EmailLibrary
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailRequest emailRequest);
    }
}
