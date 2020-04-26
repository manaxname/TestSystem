using NUnit.Framework;
using System.Threading.Tasks;
using TestSystem.Domain.Logic;
using TestSystem.Domain.Logic.Interfaces;
using TestSystem.Domain.Logic.Managers;

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

        [TestCase("email1", "paswhash1", "role1")]
        [TestCase("email2", "paswhash2", "role2")]
        [TestCase("email3", "paswhash3", "role3")]
        public async Task CreateUsers_Test(string email, string passwordHash, string role)
        {
            var domainUser = Helper.CreateDomainUser(email, passwordHash, role);

            await _userManager.CreateUserAsync(domainUser);
        }

        [TestCase("email1", "paswhash1", "role1")]
        [TestCase("email2", "paswhash2", "role2")]
        [TestCase("email3", "paswhash3", "role3")]
        public async Task GetUsersByEmail_Test(string email, string passwordHash, string role)
        {
            var domainUser = Helper.CreateDomainUser(email, passwordHash, role);

            await _userManager.CreateUserAsync(domainUser);

            var expectedUser = await _userManager.GetUserByEmailAsync(email);

            Assert.AreEqual(email, expectedUser.Email);
        }
    }
}
