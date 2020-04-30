using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Threading;
using System.Threading.Tasks;
using TestSystem.Data.Models;

namespace TestSystem.Data
{
    public class TestSystemContext : DbContext, ITestSystemContext
    {
        public TestSystemContext(DbContextOptions<TestSystemContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<UserTopic> UserTopics { get; set; }
        public DbSet<UserTest> UserTests { get; set; }
        public DbSet<UserAnswer> UserAnswers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Topic>()
              .HasMany(x => x.Tests)
              .WithOne()
              .HasForeignKey(x => x.TopicId);

            modelBuilder.Entity<Test>()
                .HasMany(x => x.Questions)
                .WithOne()
                .HasForeignKey(x => x.TestId);

            modelBuilder.Entity<Question>()
                .HasMany(x => x.Answers)
                .WithOne()
                .HasForeignKey(x => x.QuestionId);

            modelBuilder.Entity<UserAnswer>()
                .HasKey(x => new { x.UserId, x.AnswerId });
            modelBuilder.Entity<UserAnswer>()
                .HasOne<User>(x => x.User)
                .WithMany(x => x.UserAnswers)
                .HasForeignKey(x => x.UserId);
            modelBuilder.Entity<UserAnswer>()
               .HasOne<Answer>(x => x.Answer)
               .WithMany(x => x.UserAnswers)
               .HasForeignKey(x => x.AnswerId);

            modelBuilder.Entity<UserTest>()
                .HasKey(x => new { x.UserId, x.TestId });
            modelBuilder.Entity<UserTest>()
                .HasOne<User>(x => x.User)
                .WithMany(x => x.UserTests)
                .HasForeignKey(x => x.UserId);
            modelBuilder.Entity<UserTest>()
               .HasOne<Test>(x => x.Test)
               .WithMany(x => x.UserTests)
               .HasForeignKey(x => x.TestId);

            modelBuilder.Entity<UserTopic>()
              .HasKey(x => new { x.UserId, x.TopicId });
            modelBuilder.Entity<UserTopic>()
                .HasOne<User>(x => x.User)
                .WithMany(x => x.UserTopics)
                .HasForeignKey(x => x.UserId);
            modelBuilder.Entity<UserTopic>()
               .HasOne<Topic>(x => x.Topic)
               .WithMany(x => x.UserTopics)
               .HasForeignKey(x => x.TopicId);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}