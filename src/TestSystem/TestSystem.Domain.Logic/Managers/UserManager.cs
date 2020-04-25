using AutoMapper;
using System;
using TestSystem.Data;
using TestSystem.Domain.Logic.Interfaces;
using System.Linq;
using DataUser = TestSystem.Data.Models.User;
using DomainUser = TestSystem.Domain.Models.User;
using TestSystem.Domain.Models;
using TestSystem.Common;

namespace TestSystem.Domain.Logic.Managers
{
    public class UserManager : IUserManager
    {
        private readonly ITestSystemContext _tpContext;

        private IMapper _mapper;

        public UserManager(ITestSystemContext tpContext, IMapper mapper)
        {
            _tpContext = tpContext ?? throw new ArgumentNullException(nameof(tpContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public int CreateUser(string email, string password, string role)
        {
            if (this.IsUserExists(email))
            {
                // TODO: Custom Exception here.
                throw new Exception(message: "User already exists.");
            }

            var passwordHash = CryptographyHelper.GetSha256Hash(password);
            var newDomainUser = Helper.CreateDomainUser(email, passwordHash, role);
            
            var newDataUser = _mapper.Map<DataUser>(newDomainUser);

            _tpContext.Users.Add(newDataUser);

            return _tpContext.SaveChangesAsync(default).Result;
        }

        public int CreateUser(DomainUser user)
        {
            if (this.IsUserExists(user.Id))
            {
                // TODO: Custom Exception here.
                throw new Exception(message: "User already exists.");
            }

            var dataUser = _mapper.Map<DataUser>(user);

            _tpContext.Users.Add(dataUser);

            return _tpContext.SaveChangesAsync(default).Result;
        }

        public void DeleteUser(string email)
        {
            throw new NotImplementedException();
        }

        public DomainUser GetUserByEmail(string email)
        {
            if (!this.IsUserExists(email))
            {
                // TODO: Custom Exception here.
                throw new Exception(message: "User does't exist.");
            }

            var dataUser = _tpContext.Users.First(user => user.Email == email);

            var domainUser = _mapper.Map<DomainUser>(dataUser);

            return domainUser;
        }

        public DomainUser GetUserById(int id)
        {
            if(!this.IsUserExists(id))
            {
                // TODO: Custom Exception here.
                throw new Exception(message: "User does't exist.");
            }

            var dataUser = _tpContext.Users.First(user => user.Id == id);

            var domainUser = _mapper.Map<DomainUser>(dataUser);

            return domainUser;
        }

        public int GetUserId(string email)
        {
            return GetUserByEmail(email).Id;
        }

        public bool IsUserExists(int id)
        {
            return _tpContext.Users.Any(user => user.Id == id);
        }

        public bool IsUserExists(string email)
        {
            return _tpContext.Users.Any(user => user.Email == email);
        }

        public bool ValidateUserPassword(DomainUser user, string userPassword)
        {
            var userPasswordHash = CryptographyHelper.GetSha256Hash(userPassword);

            return user.PasswordHash.Equals(userPasswordHash, StringComparison.Ordinal);
        }
    }
}
