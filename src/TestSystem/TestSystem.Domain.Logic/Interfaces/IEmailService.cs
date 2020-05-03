using System.Threading.Tasks;

namespace TestSystem.Domain.Logic.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string senderName, string senderEmail, string senderEmailPassword,
            string smtpHost, int smtpPort, string recipientEmail, string subject, string message);
    }
}