using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TestSystem.Data;
using TestSystem.Domain.Logic.Interfaces;
using TestSystem.Domain.Logic.Managers;
using Microsoft.EntityFrameworkCore;
using DataTest = TestSystem.Data.Models.Test;
using DomainTest = TestSystem.Domain.Models.Test;
using DataUserTest = TestSystem.Data.Models.UserTest;
using DomainUserTest = TestSystem.Domain.Models.UserTest;
using DataUser = TestSystem.Data.Models.User;
using DomainUser = TestSystem.Domain.Models.User;
using Xunit;
using TestSystem.Domain.Logic.Mappers;
using Bogus;
using TestSystem.Domain.Logic;
using TestSystem.Common;
using System.Linq;
using Moq;
using TestSystem.Common.CustomExceptions;

namespace TestSystem.Logic.Tests.TestManagerTests
{
    public class TestManagerTests
    {
        private readonly IMapper _mapper;

        private readonly Faker<DataTest> _dataTests;

        private readonly Faker<DataUserTest> _dataUserTests;

        private readonly Faker<DataUser> _dataUsers;

        private readonly int _generateCount = 20;

        private readonly int _generateTestNameLength = 10;

        private readonly int _generateTestMinMinutes = 1;

        private readonly int _generateTestMaxMinutes = 10;

        public TestManagerTests()
        {
            var dataDomainProfile = new DataDomainProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(dataDomainProfile));
            _mapper = new Mapper(configuration);

            _dataTests = new Faker<DataTest>()
                .RuleFor(x => x.Name, y => y.Internet.Random.AlphaNumeric(_generateTestNameLength))
                .RuleFor(x => x.Minutes, y => y.Random.Number(_generateTestMinMinutes, _generateTestMaxMinutes))
                .RuleFor(x => x.IsDeleted, y => y.Random.Bool());

            _dataUsers = new Faker<DataUser>()
                .RuleFor(x => x.Email, y => y.Internet.Email())
                .RuleFor(x => x.PasswordHash, y => CryptographyHelper.GetSha256Hash(y.Internet.Password()))
                .RuleFor(x => x.Role, y => y.PickRandom<UserRoles>())
                .RuleFor(x => x.IsDeleted, y => y.Random.Bool())
                .RuleFor(x => x.ConfirmationToken, y => Guid.NewGuid())
                .RuleFor(x => x.IsConfirmed, y => y.Random.Bool());

            _dataUserTests = new Faker<DataUserTest>()
                .RuleFor(x => x.IsDeleted, y => y.Random.Bool())
                .RuleFor(x => x.StartTime, y => new DateTime())
                .RuleFor(x => x.Points, y => y.Random.Number(1, 20))
                .RuleFor(x => x.Status, y => y.PickRandom<TestStatus>());
        }

        [Fact]
        public async Task GetTestByIdAsync_Test()
        {
            IList<DataTest> dataTests = _dataTests.Generate(_generateCount).ToList();

            var options = new DbContextOptionsBuilder<TestSystemContext>()
                .UseInMemoryDatabase(databaseName: "TestManagerTests_GetTestByIdAsync_TestDatabase")
                .Options;

            using (var dbContext = new TestSystemContext(options))
            {
                dbContext.Tests.AddRange(dataTests);
                dbContext.SaveChanges();
            }

            using (var dbContext = new TestSystemContext(options))
            {
                var userManagerMock = new Mock<IUserManager>();

                var sut = new TestManager(dbContext, userManagerMock.Object, _mapper);

                foreach (var dataTest in dataTests)
                {
                    var domainTest = await sut.GetTestByIdAsync(dataTest.Id);

                    Assert.Equal(domainTest.Id, dataTest.Id);
                }
            }
        }

        [Fact]
        public async Task GetTestByIdAsync_Test_ThrowTestNotFoundException()
        {
            int searchedId = 1;

            var options = new DbContextOptionsBuilder<TestSystemContext>()
                .UseInMemoryDatabase(databaseName: "TestManagerTests_GetTestByIdAsync_Test_ThrowTestNotFoundException_TestDatabase")
                .Options;

            using (var dbContext = new TestSystemContext(options))
            {
                var userManagerMock = new Mock<IUserManager>();

                var sut = new TestManager(dbContext, userManagerMock.Object, _mapper);

                await Assert.ThrowsAsync<TestNotFoundException>(() => sut.GetTestByIdAsync(searchedId));
            }
        }
    }
}
