using QuizManager.Models.Permission;
using System.Collections.Generic;

namespace QuizManager.Models.ViewModels
{
    public class DetailsViewModel
    {
        public int QuizId { get; set; }
        public string QuizName { get; set; }
        public List<Question> Questions { get; set; }
        public Permissions UserPermission { get; set; }
    }
}
