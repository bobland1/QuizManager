using System.Collections.Generic;

namespace QuizManager.Models.ViewModels
{
    public class CreateViewModel
    {
        public string QuizName { get; set; }
        public List<Question> Questions { get; set; }
    }
}
