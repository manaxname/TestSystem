using Microsoft.EntityFrameworkCore;
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
        public DbSet<AnswerOption> AnswersOptions { get; set; }
        public DbSet<AnswerText> AnswersTexts { get; set; }
        public DbSet<UserAnswerOption> UserAnswerOptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Test>()
                .HasMany(x => x.Questions)
                .WithOne()
                .HasForeignKey(x => x.TestId);

            modelBuilder.Entity<Question>()
                .HasMany(x => x.AnswersOption)
                .WithOne()
                .HasForeignKey(x => x.QuestionId);

            modelBuilder.Entity<Question>()
                .HasOne<AnswerText>(x => x.AnswersText)
                .WithOne(x => x.Question)
                .HasForeignKey<AnswerText>(x => x.QuestionId);

            modelBuilder.Entity<UserAnswerOption>()
                .HasOne<User>(x => x.User)
                .WithMany(x => x.UserAnswerOptions)
                .HasForeignKey(x => x.UserId);
            modelBuilder.Entity<UserAnswerOption>()
               .HasOne<AnswerOption>(x => x.AnswerOption)
               .WithMany(x => x.UserAnswerOptions)
               .HasForeignKey(x => x.AnswerOptionId);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var result = base.SaveChanges();
            return Task.FromResult(result);
            //return base.SaveChangesAsync(cancellationToken);
        }
    }
}