using MailKit.Net.Proxy;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace EmailLibrary
{
    public class MailService : IEmailService
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

            if (!string.IsNullOrEmpty(mailRequest.CcEmail))
                message.Cc.Add(MailboxAddress.Parse(mailRequest.CcEmail));

            var builder = new BodyBuilder();

            // Set the plain-text version of the message text
            builder.TextBody = mailRequest.Body;
            builder.HtmlBody = mailRequest.HtmlBody;

            // We may also want to attach a calendar event for Monica's party...
            if (mailRequest.Attachments != null)
            {
                foreach (var file in mailRequest.Attachments)
                {
                    if (file.Stream != null)
                        builder.Attachments.Add(file.FileName, file.Stream);
                    else
                    {
                        if (file.Data != null)
                            builder.Attachments.Add(file.FileName, file.Data);
                        else
                            builder.Attachments.Add(file.FileName);
                    }
                    //if (file.Length > 0)
                }
            }

            // Now we just need to set the message body and we're done            
            message.Body = builder.ToMessageBody();            

            // send email
            using (var client = new SmtpClient())
            {
                client.CheckCertificateRevocation = false;

                if (!string.IsNullOrWhiteSpace(_mailSettings.ProxyHost))
                    client.ProxyClient = new HttpProxyClient(_mailSettings.ProxyHost, _mailSettings.ProxyPort);   // <-- set proxy
                client.Connect(_mailSettings.Host, _mailSettings.Port, false);

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(_mailSettings.Mail, _mailSettings.Password);

                var result = await client.SendAsync(message);
                Console.WriteLine($"email result: {result}");
                client.Disconnect(true);
            }
        }
    }
}
