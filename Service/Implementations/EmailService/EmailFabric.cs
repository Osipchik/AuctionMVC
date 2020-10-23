using System.Threading.Tasks;
using EmailService;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Service.Implementations.EmailService
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
        
        public ValueTask<MimeMessage> BuildEmailValueTask(MailMessage message)
        {
            return message.EmailType switch
            {
                EmailTypes.ConfirmEmail => BuildConfirmEmail(message),
                // EmailTypes.ResetPassword => BuildPasswordResetEmail(message)
            };
        }
        
        private async ValueTask<MimeMessage> BuildConfirmEmail(MailMessage message)
        {
            var subject = _configuration.GetSection("EmailSubjects")["Email"];
            var body = await _renderer.RenderViewToStringAsync("/Views/Emails/ConfirmAccountEmail.cshtml", message);

            return BuildEmail(message.Name, message.To, subject, body);
        }

        // private async ValueTask<MimeMessage> BuildPasswordResetEmail(MailMessage message)
        // {
        //     const string from = Constants.Identity;
        //     const string subject = Constants.ResetPasswordSubject;
        //     var body = await _renderer.RenderViewToStringAsync(Constants.ResetPasswordBody, model);
        //
        //     return BuildEmail(message.Name, message.To, from, subject, body);
        // }
        
        private MimeMessage BuildEmail(string name, string to, string subject, string body)
        {
            var from = _configuration.GetSection("Admin")["Email"];
            var user = "user";
            
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(from, user));
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