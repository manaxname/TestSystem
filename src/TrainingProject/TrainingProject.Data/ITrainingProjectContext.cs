using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
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
        DbSet<Answer> Answers { get; set; }
        DbSet<UserAnswer> UserAnswers { get; set; }
        DbSet<UserTest> UserTests { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken); 
    }
}
