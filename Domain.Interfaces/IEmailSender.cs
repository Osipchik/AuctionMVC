using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IEmailSender<TMessage> where TMessage : class
    {
        Task Send(TMessage message, string subject = null);
    }
}