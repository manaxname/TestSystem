using AutoMapper;
using System;
using TestSystem.Data;
using TestSystem.Domain.Logic.Interfaces;
using System.Linq;
using DataUser = TestSystem.Data.Models.User;
using DomainUser = TestSystem.Domain.Models.User;
using TestSystem.Domain.Models;
using TestSystem.Common;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestSystem.Common.CustomExceptions;

namespace TestSystem.Domain.Logic.Managers
{
    public class UserManager : IUserManager
    {
        private readonly ITestSystemContext _dbContext;

        private IMapper _mapper;

        public UserManager(ITestSystemContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<DomainUser> CreateUserAsync(string email, string password, string role)
        {
            await ThrowIfUserAlreadyExistsAsync(email);

            var passwordHash = CryptographyHelper.GetSha256Hash(password);
            var domainUser = Helper.CreateDomainUser(email, passwordHash, role);
            var dataUser = _mapper.Map<DataUser>(domainUser);

            _dbContext.Users.Add(dataUser);
            await _dbContext.SaveChangesAsync(default);

            return _mapper.Map<DomainUser>(dataUser);
        }

        public async Task<DomainUser> CreateUserAsync(DomainUser user)
        {
            await ThrowIfUserAlreadyExistsAsync(user.Email);

            var dataUser = _mapper.Map<DataUser>(user);

            _dbContext.Users.Add(dataUser);

            _dbContext.Users.Add(dataUser);
            await _dbContext.SaveChangesAsync(default);

            return _mapper.Map<DomainUser>(dataUser);
        }

        public async Task DeleteUserAsync(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<DomainUser> GetUserByEmailAsync(string email)
        {
            var dataUser = await _dbContext.Users.FirstOrDefaultAsync(user => user.Email == email);

            if (dataUser == null)
            {
                throw new UserNotFoundException(email);
            }

            var domainUser = _mapper.Map<DomainUser>(dataUser);

            return domainUser;
        }

        public async Task<DomainUser> GetUserByIdAsync(int id)
        {
            var dataUser = await _dbContext.Users.FirstOrDefaultAsync(user => user.Id == id);

            if (dataUser == null)
            {
                throw new UserNotFoundException(id.ToString());
            }

            var domainUser = _mapper.Map<DomainUser>(dataUser);

            return domainUser;
        }

        public async Task<int> GetUserIdAsync(string email)
        {
            return (await GetUserByEmailAsync(email)).Id;
        }

        public async Task<bool> IsUserExistsAsync(int id)
        {
            return await _dbContext.Users.AnyAsync(user => user.Id == id);
        }

        public async Task<bool> IsUserExistsAsync(string email)
        {
            return await _dbContext.Users.AnyAsync(user => user.Email == email);
        }

        public async Task ThrowIfUserAlreadyExistsAsync(string email)
        {
            if (await IsUserExistsAsync(email))
            {
                throw new UserAlreadyExistsException(email);
            }
        }

        public async Task ThrowIfUserAlreadyExistsAsync(int id)
        {
            if (await IsUserExistsAsync(id))
            {
                throw new UserAlreadyExistsException(id.ToString());
            }
        }

        public async Task ThrowIfUserNotExistsAsync(string email)
        {
            if (! await IsUserExistsAsync(email))
            {
                throw new UserNotFoundException(email);
            }
        }

        public async Task ThrowIfUserNotExistsAsync(int id)
        {
            if (!await IsUserExistsAsync(id))
            {
                throw new UserNotFoundException(id.ToString());
            }
        }

        public async Task UpdateUserConfirmStatus(int userId, bool isConfirmed)
        {
            DataUser dataUser = await _dbContext.Users.FirstAsync(x => x.Id == userId);

            dataUser.IsConfirmed = isConfirmed;

            await _dbContext.SaveChangesAsync(default);
        }

        public async Task UpdateUserConfirmationToken(int userId, Guid confgirmToken)
        {
            DataUser dataUser = await _dbContext.Users.FirstAsync(x => x.Id == userId);

            dataUser.ConfirmationToken = confgirmToken;

            await _dbContext.SaveChangesAsync(default);
        }

        public bool ValidateUserPassword(DomainUser user, string userPassword)
        {
            var userPasswordHash = CryptographyHelper.GetSha256Hash(userPassword);

            return user.PasswordHash.Equals(userPasswordHash, StringComparison.Ordinal);
        }
    }
}
