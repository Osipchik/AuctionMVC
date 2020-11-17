using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Service.Interfaces;

namespace Web.EmailSender
{
    public class EmailFabric
    {
        private readonly IRazorViewToStringRenderer _renderer;
        private readonly IConfiguration _configuration;
        
        public EmailFabric(IRazorViewToStringRenderer renderer, IConfiguration configuration)
        {
            _renderer = renderer;
            _configuration = configuration;
        }
        
        public ValueTask<MimeMessage> BuildEmailValueTask(MailMessage message, string subject, string fromEmail)
        {
            return message.EmailType switch
            {
                EmailTypes.ConfirmEmail => BuildConfirmEmail(message, fromEmail),
                EmailTypes.LaunchNotification => BuildLaunchNotification(message, fromEmail),
                EmailTypes.FinishNotification => BuildFinishNotification(message, fromEmail),
                // EmailTypes.ResetPassword => BuildPasswordResetEmail(message)
                _ => BuildMessage(message, subject, fromEmail)
            };
        }

        private async ValueTask<MimeMessage> BuildFinishNotification(MailMessage message, string fromEmail)
        {
            var subject = _configuration.GetSection("EmailSubjects")["Launch"];
            var body = await _renderer.RenderViewToStringAsync("/Views/Emails/FinishNotificationEmail.cshtml", message);

            return BuildEmail(message.Name, message.To, subject, body, fromEmail);
        }
        
        private async ValueTask<MimeMessage> BuildLaunchNotification(MailMessage message, string fromEmail)
        {
            var subject = _configuration.GetSection("EmailSubjects")["Launch"];
            var body = await _renderer.RenderViewToStringAsync("/Views/Emails/LaunchNotificationEmail.cshtml", message);

            return BuildEmail(message.Name, message.To, subject, body, fromEmail);
        }
        
        private async ValueTask<MimeMessage> BuildConfirmEmail(MailMessage message, string fromEmail)
        {
            var subject = _configuration.GetSection("EmailSubjects")["Email"];
            var body = await _renderer.RenderViewToStringAsync("/Views/Emails/ConfirmAccountEmail.cshtml", message);

            return BuildEmail(message.Name, message.To, subject, body, fromEmail);
        }
        
        private async ValueTask<MimeMessage> BuildMessage(MailMessage message, string subject, string fromEmail)
        {
            var body = await _renderer.RenderViewToStringAsync("/Views/Emails/MessageEmail.cshtml", message);

            return BuildEmail(message.Name, message.To, subject, body, fromEmail);
        }

        private MimeMessage BuildEmail(string name, string to, string subject, string body, string fromEmail)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_configuration.GetSection("EmailTitle")["Confirm"], fromEmail));
            emailMessage.To.Add(new MailboxAddress(name, to));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = body
            };
            
            return emailMessage;
        }
    }
}