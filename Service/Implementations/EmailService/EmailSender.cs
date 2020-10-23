using System.Threading.Tasks;
using EmailService;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Service.Interfaces;


namespace Service.Implementations.EmailService
{
    public class EmailSender : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly EmailFabric _emailFabric;

        public EmailSender(IConfiguration configuration, IRazorViewToStringRenderer renderer)
        {
            _configuration = configuration;
            _emailFabric = new EmailFabric(renderer, configuration);
        }


        public async Task Send(MailMessage message)
        {
            using (var client = new SmtpClient())
            {
                var authConfig = _configuration.GetSection("Admin");

                var res = await _emailFabric.BuildEmailValueTask(message);
                
                await client.ConnectAsync("smtp.gmail.com", 587, false);
                await client.AuthenticateAsync(authConfig["Email"], authConfig["password"]);
                await client.SendAsync(res);
 
                await client.DisconnectAsync(true);
            }
        }
    }
}