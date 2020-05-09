using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TestSystem.Data;
using TestSystem.Domain.Logic.Interfaces;
using TestSystem.Domain.Logic.Managers;
using TestSystem.Domain.Models;
using Microsoft.EntityFrameworkCore;
using DataUser = TestSystem.Data.Models.User;
using DomainUser = TestSystem.Domain.Models.User;
using Xunit;
using TestSystem.Domain.Logic.Mappers;
using Bogus;
using TestSystem.Domain.Logic;
using TestSystem.Common;
using System.Diagnostics;
using System.Linq;

namespace TestSystem.Logic.Tests.UserManagerTests
{
    public class TestManagerTests
    {
        private readonly IMapper _mapper;

        private readonly Faker<DataUser> _dataUsers;

        private readonly int _generateCount = 20;

        public TestManagerTests()
        {
            var dataDomainProfile = new DataDomainProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(dataDomainProfile));
            _mapper = new Mapper(configuration);
            
            _dataUsers = new Faker<DataUser>()
                .RuleFor(x => x.Email, y => y.Internet.Email())
                .RuleFor(x => x.PasswordHash, y => CryptographyHelper.GetSha256Hash(y.Internet.Password()))
                .RuleFor(x => x.Role, y => y.PickRandom<UserRoles>())
                .RuleFor(x => x.IsDeleted, y => y.Random.Bool())
                .RuleFor(x => x.ConfirmationToken, y => Guid.NewGuid())
                .RuleFor(x => x.IsConfirmed, y => y.Random.Bool());
        }

        [Fact]
        public async Task GetUserByIdAsync_Test()
        {
            IList<DataUser> dataUsers = _dataUsers.Generate(_generateCount).ToList();

            var options = new DbContextOptionsBuilder<TestSystemContext>()
                .UseInMemoryDatabase(databaseName: "UserManagerTests_GetUserByIdAsync_TestDatabase")
                .Options;

            using (var dbContext = new TestSystemContext(options))
            {
                dbContext.Users.AddRange(dataUsers);
                dbContext.SaveChanges();
            }

            using (var dbContext = new TestSystemContext(options))
            {
                var sut = new UserManager(dbContext, _mapper);

                foreach (var dataUser in dataUsers)
                {
                    var domainUser = await sut.GetUserByIdAsync(dataUser.Id);

                    Assert.Equal(domainUser.Id, dataUser.Id);
                }
            }
        }
    }
}
