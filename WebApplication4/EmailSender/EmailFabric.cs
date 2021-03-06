﻿using System.Threading.Tasks;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using MimeKit;


namespace WebApplication4.EmailSender
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
                EmailTypes.FinishNotification => BuildFinishNotification(message, fromEmail),
                EmailTypes.WinNotification => BuildWinNotification(message, fromEmail),
                _ => BuildMessage(message, subject, fromEmail)
            };
        }

        private async ValueTask<MimeMessage> BuildFinishNotification(MailMessage message, string fromEmail)
        {
            var subject = _configuration.GetSection("EmailSubjects")["Finish"];
            var body = await _renderer.RenderViewToStringAsync("/Views/Emails/FinishNotificationEmail.cshtml", message);

            return BuildEmail(message.Name, message.To, subject, body, fromEmail);
        }
        
        private async ValueTask<MimeMessage> BuildWinNotification(MailMessage message, string fromEmail)
        {
            var subject = _configuration.GetSection("EmailSubjects")["Win"];

            message.Message = "You won this:" + message.Message;
            var body = await _renderer.RenderViewToStringAsync("/Views/Emails/WinNotification.cshtml", message);

            return BuildEmail(message.Name, message.To, subject, body, fromEmail);
        }
        
        private async ValueTask<MimeMessage> BuildConfirmEmail(MailMessage message, string fromEmail)
        {
            var subject = _configuration.GetSection("EmailSubjects")["Message"];
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