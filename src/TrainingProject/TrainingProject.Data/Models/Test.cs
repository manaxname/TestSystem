using System.Collections.Generic;

namespace TrainingProject.Data.Models
{
    public class Test
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Question> Questions { get; set; }
    }
}
