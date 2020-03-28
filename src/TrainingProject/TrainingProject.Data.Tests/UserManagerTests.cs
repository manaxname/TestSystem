using NUnit.Framework;
using TrainingProject.Data.Models;
using TrainingProject.Domain.Logic;
using TrainingProject.Domain.Logic.Interfaces;
using TrainingProject.Domain.Logic.Managers;
using TrainingProject.Domain.Models;
using DataUser = TrainingProject.Data.Models.User;
using DomainUser = TrainingProject.Domain.Models.User;

namespace TrainingProject.Data.Tests
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
