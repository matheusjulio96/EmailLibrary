using MailKit.Net.Proxy;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Threading.Tasks;

namespace EmailLibrary
{
    public class MailService : IMailService
    {
        private readonly EmailSettings _mailSettings;
        public MailService(IOptions<EmailSettings> emailSettings)
        {
            _mailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(EmailRequest mailRequest)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
            message.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            message.Subject = mailRequest.Subject;

            var builder = new BodyBuilder();

            // Set the plain-text version of the message text
            builder.TextBody = mailRequest.Body;

            // We may also want to attach a calendar event for Monica's party...
            if (mailRequest.Attachments != null)
            {
                foreach (var file in mailRequest.Attachments)
                {
                    if (file.Length > 0)
                    {
                        builder.Attachments.Add(file.FileName);
                    }
                }
            }

            // Now we just need to set the message body and we're done            
            message.Body = builder.ToMessageBody();

            // send email
            using (var client = new SmtpClient())
            {
                if (!string.IsNullOrWhiteSpace(_mailSettings.ProxyHost))
                    client.ProxyClient = new HttpProxyClient(_mailSettings.ProxyHost, _mailSettings.ProxyPort);   // <-- set proxy
                client.Connect(_mailSettings.Host, _mailSettings.Port, false);

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(_mailSettings.Mail, _mailSettings.Password);

                await client.SendAsync(message);
                client.Disconnect(true);
            }
        }
    }
}
