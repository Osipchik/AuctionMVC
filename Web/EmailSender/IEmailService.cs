using System.Threading.Tasks;

namespace Web.EmailSender
{
    public interface IEmailService
    {
        Task Send(MailMessage message);
    }
}