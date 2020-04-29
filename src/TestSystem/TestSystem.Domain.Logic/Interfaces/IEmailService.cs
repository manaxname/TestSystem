using System.Threading.Tasks;

namespace TestSystem.Domain.Logic.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}