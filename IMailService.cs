using System.Threading.Tasks;

namespace EmailLibrary
{
    public interface IMailService
    {
        Task SendEmailAsync(EmailRequest emailRequest);
    }
}
