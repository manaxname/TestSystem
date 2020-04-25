using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TestSystem.Data.Models;
using TestSystem.Domain.Logic.Mappers;

namespace TestSystem.Data.Tests
{
    [TestFixture]
    internal abstract class TestBase
    {
        private string ConnectionString { get; set; }

        protected TestSystemContext DbContext { get; }

        protected IMapper Mapper { get; }

        protected TestBase()
        {
            ConnectionString = Configuration.Get("DefaultConnection");
            var optionsBuilder = new DbContextOptionsBuilder<TestSystemContext>();
            optionsBuilder.UseSqlServer(ConnectionString);
            DbContext = new TestSystemContext(optionsBuilder.Options);

            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<DataDomainProfile>();
            });
            Mapper = config.CreateMapper();

            DbContext.Database.EnsureCreated();
        }

        [OneTimeTearDown]
        public void RunAfterAllTests()
        {
            DbContext.Dispose();
        }

        [SetUp]
        public void RunBeforeEachTest()
        {
            ClearAllTables();
        }

        [TearDown]
        public void RunBeforeAfterTest()
        {
            ClearAllTables();
        }

        private void ClearAllTables()
        {
            DbContext.Set<User>().RemoveRange(DbContext.Set<User>());
            DbContext.SaveChanges();
        }
    }
}