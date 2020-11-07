using System.Collections.Generic;

namespace QuizManager.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Question> Questions { get; set; }
    }
}
