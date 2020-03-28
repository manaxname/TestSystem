using TrainingProject.Domain.Models;

namespace TrainingProject.Domain.Logic.Interfaces
{
    public interface IUserManager
    {
        int CreateUser(string email, string password, string role);
        int CreateUser(User user);
        void DeleteUser(string email);
        User GetUserByEmail(string email);
        User GetUserById(int id);
        bool IsUserExists(int id);
        bool IsUserExists(string email);
        int GetUserId(string email);
        bool ValidateUserPassword(User user, string userPassword);
    }
}
