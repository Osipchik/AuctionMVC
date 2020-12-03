using System.Threading.Tasks;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using MailKit.Net.Smtp;

namespace WebApplication4.EmailSender
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


        public async Task Send(MailMessage message, string subject = "")
        {
            var config = _configuration.GetSection("EmailSender");

            var res = await _emailFabric.BuildEmailValueTask(message, subject, config["Email"]);

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(config["Host"], int.Parse(config["Port"]), bool.Parse(config["UseSSL"]));
                await client.AuthenticateAsync(config["Email"], config["Password"]);
                await client.SendAsync(res);

                await client.DisconnectAsync(true);
            }
        }
    }
}