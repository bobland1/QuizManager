using System.Collections.Generic;

namespace QuizManager.Models.ViewModels
{
    public class EditViewModel
    {
        public int QuizId { get; set; }
        public string QuizName { get; set; }
        public List<Question> Questions { get; set; }
    }
}
