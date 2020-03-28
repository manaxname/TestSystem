using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrainingProject.Data.Models;

namespace TrainingProject.Data
{
    public interface ITrainingProjectContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Test> Tests { get; set; }
        DbSet<Question> Questions { get; set; }
        DbSet<AnswerOption> AnswersOptions { get; set; }
        DbSet<AnswerText> AnswersTexts { get; set; }
        DbSet<UserAnswerOption> UserAnswerOptions { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
