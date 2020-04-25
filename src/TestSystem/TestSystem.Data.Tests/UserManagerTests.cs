using NUnit.Framework;
using TestSystem.Data.Models;
using TestSystem.Domain.Logic;
using TestSystem.Domain.Logic.Interfaces;
using TestSystem.Domain.Logic.Managers;
using TestSystem.Domain.Models;
using DataUser = TestSystem.Data.Models.User;
using DomainUser = TestSystem.Domain.Models.User;

namespace TestSystem.Data.Tests
{
    [TestFixture]
    internal class UserManagerTests : TestBase
    {
        private readonly IUserManager _userManager;

        public UserManagerTests()
        {
            _userManager = new UserManager(base.DbContext, base.Mapper);
        }

        [TestCase("email1", "pasw1", "role1")]
        [TestCase("email2", "pasw2", "role2")]
        [TestCase("email3", "pasw3", "role3")]
        public void CreateUsers_Test(string email, string passwordHash, string role)
        {
            var domainUser = Helper.CreateDomainUser(email, passwordHash, role);

            _userManager.CreateUser(domainUser);
        }

        [TestCase("email1", "pasw1", "role1")]
        [TestCase("email2", "pasw2", "role2")]
        [TestCase("email3", "pasw3", "role3")]
        public void GetUsersByEmail_Test(string email, string passwordHash, string role)
        {
            var domainUser = Helper.CreateDomainUser(email, passwordHash, role);

            _userManager.CreateUser(domainUser);

            var expectedUser = _userManager.GetUserByEmail(email);

            Assert.AreEqual(email, expectedUser.Email);
        }
    }
}
