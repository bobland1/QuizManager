using QuizManager.Models.Permission;
using System.Collections.Generic;

namespace QuizManager.Models.ViewModels
{
    public class IndexViewModel
    {
        public List<Quiz> Quizzes { get; set; }
        public Permissions UserPermission { get; set; }
    }
}
