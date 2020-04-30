﻿using System.Threading.Tasks;
using TestSystem.Domain.Models;

namespace TestSystem.Domain.Logic.Interfaces
{
    public interface IUserManager
    {
        Task<int> CreateUserAsync(string email, string password, string role);
        Task<int> CreateUserAsync(User user);
        Task DeleteUserAsync(string email);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByIdAsync(int id);
        Task<bool> IsUserExistsAsync(int id);
        Task<bool> IsUserExistsAsync(string email);
        Task<int> GetUserIdAsync(string email);
        bool ValidateUserPassword(User user, string userPassword);
        Task ThrowIfUserNotExistsAsync(string email);
        Task ThrowIfUserNotExistsAsync(int id);
        Task ThrowIfUserAlreadyExistsAsync(string email);
        Task ThrowIfUserAlreadyExistsAsync(int id);
    }
}