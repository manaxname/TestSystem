using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Threading;
using System.Threading.Tasks;
using TrainingProject.Data.Models;

namespace TrainingProject.Data
{
    public class TrainingProjectContext : DbContext, ITrainingProjectContext
    {
        public TrainingProjectContext(DbContextOptions<TrainingProjectContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<UserAnswer> UserAnswers { get; set; }
        public DbSet<UserTest> UserTests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}