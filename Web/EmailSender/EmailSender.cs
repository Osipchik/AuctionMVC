using System.Threading.Tasks;
using Domain.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;

namespace Web.EmailSender
{
    public class EmailSender : IEmailSender<MailMessage>
    {
        private readonly IConfiguration _configuration;
        private readonly EmailFabric _emailFabric;

        public EmailSender(IConfiguration configuration, IRazorViewToStringRenderer renderer)
        {
            _configuration = configuration;
            _emailFabric = new EmailFabric(renderer, configuration);
        }


        public async Task Send(MailMessage message, string subject = null)
        {
            using (var client = new SmtpClient())
            {
                var authConfig = _configuration.GetSection("Admin");

                var res = await _emailFabric.BuildEmailValueTask(message, subject, authConfig["Email"]);
                
                await client.ConnectAsync("smtp.gmail.com", 587, false);
                await client.AuthenticateAsync(authConfig["Email"], authConfig["EmailPassword"]);
                await client.SendAsync(res);
 
                await client.DisconnectAsync(true);
            }
        }
    }
}