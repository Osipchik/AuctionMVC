namespace WebApplication4.EmailSender
{
    public class MailMessage
    {
        public string To { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public EmailTypes EmailType { get; set; }
        
        public MailMessage(string to, string name,  string message, EmailTypes type)
        {
            To = to;
            Name = name;
            Message = message;
            EmailType = type;
        }
    }
}