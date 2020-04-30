using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestSystem.Data.Models;

namespace TestSystem.Data
{
    public interface ITestSystemContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Topic> Topics { get; set; }
        DbSet<Test> Tests { get; set; }
        DbSet<Question> Questions { get; set; }
        DbSet<Answer> Answers { get; set; }
        DbSet<UserTopic> UserTopics { get; set; }
        DbSet<UserTest> UserTests { get; set; }
        DbSet<UserAnswer> UserAnswers { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken); 
    }
}
