using A_MicrosoftAspNetCoreIdentityManagement.Configurations;
using MailKit.Net.Smtp;
using MimeKit;

namespace A_MicrosoftAspNetCoreIdentityManagement.Services
{
    public static class EmailService
    {
        public async static Task SendEmailAsync(MailRequest mailRequest)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(EmailConfiguration.SenderEmail);
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = mailRequest.Subject;
            var builder = new BodyBuilder();
            if (mailRequest.Attachments != null)
            {
                byte[] fileBytes;
                foreach (var file in mailRequest.Attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }
                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }
            builder.HtmlBody = mailRequest.Body;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(EmailConfiguration.Host, EmailConfiguration.Port, EmailConfiguration.UseSsl);
            smtp.Authenticate(EmailConfiguration.SenderEmail, EmailConfiguration.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
    public class MailRequest
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<IFormFile> Attachments { get; set; }
    }
}
