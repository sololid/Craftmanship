using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

namespace Craftmanship.Core.Utility
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailToSend = new MimeMessage();
            emailToSend.From.Add(MailboxAddress.Parse("example@crafmanship.se"));
            emailToSend.To.Add(MailboxAddress.Parse(email));
            emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };

            using (var emailClient = new SmtpClient()) 
            {
                emailClient.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                emailClient.Authenticate("gmail@gmail.com", "gmail");
                emailClient.SendAsync(emailToSend);
                emailClient.Disconnect(true);
            }

            return Task.CompletedTask;  
        }
    }
}
