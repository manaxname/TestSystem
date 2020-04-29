using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestSystem.Data.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public bool IsDeleted { get; set; }

        public ICollection<UserAnswer> UserAnswers { get; set; }
        public ICollection<UserTest> UserTests { get; set; }
    }
}
