using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestSystem.Common;
using TestSystem.Domain.Models;

namespace TestSystem.Domain.Logic.Interfaces
{
    public interface IUserManager
    {
        Task<User> CreateUserAsync(string email, string password, UserRoles role);
        Task<User> CreateUserAsync(User user);
        Task DeleteUserAsync(string email);
        Task UpdateUserConfirmStatus(int userId, bool isConfirmed);
        Task UpdateUserConfirmationToken(int userId, Guid confgirmToken);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByIdAsync(int id);
        Task<bool> IsUserExistsAsync(int id);
        Task<bool> IsUserExistsAsync(string email);
        Task<int> GetUsersCountAsync(string search, UserRoles userRole = UserRoles.User, CancellationToken cancellationToken = default);
        IQueryable<User> GetUsersAsync(string search, int? fromIndex = null, int? toIndex = null, UserRoles userRole = UserRoles.User);
        Task<int> GetUserIdAsync(string email);
        bool ValidateUserPassword(User user, string userPassword);
        Task ThrowIfUserNotExistsAsync(string email);
        Task ThrowIfUserNotExistsAsync(int id);
        Task ThrowIfUserAlreadyExistsAsync(string email);
        Task ThrowIfUserAlreadyExistsAsync(int id);
    }
}
