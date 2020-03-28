using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TrainingProject.Data.Models;
using TrainingProject.Domaim.Logic.Tests;
using TrainingProject.Domain.Logic.Mappers;

namespace TrainingProject.Data.Tests
{
    [TestFixture]
    internal abstract class TestBase
    {
        private string ConnectionString { get; set; }

        protected TrainingProjectContext DbContext { get; }

        protected IMapper Mapper { get; }

        protected TestBase()
        {
            ConnectionString = Configuration.Get("DefaultConnection");
            var optionsBuilder = new DbContextOptionsBuilder<TrainingProjectContext>();
            optionsBuilder.UseSqlServer(ConnectionString);
            DbContext = new TrainingProjectContext(optionsBuilder.Options);

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