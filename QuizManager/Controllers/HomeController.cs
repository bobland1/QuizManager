using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QuizManager.Controllers.Methods;
using QuizManager.Models;
using QuizManager.Models.Permission;
using QuizManager.Models.ViewModels;

namespace QuizManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private static List<Quiz> _quizzes;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
                return Redirect("/Login");

            _quizzes = new DatabaseInteraction(_configuration).FetchQuizzesFromDatabase();

            var vm = new IndexViewModel()
            {
                Quizzes = _quizzes
            };

            vm.UserPermission = User.IsInRole("Admin") ?
                Permissions.Edit : User.IsInRole("Moderator") ?
                Permissions.View : Permissions.Restricted;

            return View(vm);
        }

        public IActionResult Details(int Id)
        {
            var quizzes = _quizzes.Where(q => q.Id == Id).Single();
            var vm = new DetailsViewModel()
            {
                QuizId = quizzes.Id,
                QuizName = quizzes.Name,
                Questions = quizzes.Questions
            };

            vm.UserPermission = User.IsInRole("Admin") ?
                Permissions.Edit : User.IsInRole("Moderator") ?
                Permissions.View : Permissions.Restricted;

            return View(vm);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int Id)
        {
            var quizzes = _quizzes.Where(q => q.Id == Id).Single();
            var vm = new EditViewModel()
            {
                QuizId = Id,
                QuizName = quizzes.Name,
                Questions = quizzes.Questions
            };
            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(EditViewModel viewModel)
        {
            var quiz = _quizzes.Where(q => q.Id == viewModel.QuizId).FirstOrDefault();
            _quizzes.Remove(quiz);

            var updatedQuiz = new Quiz()
            {
                Id = viewModel.QuizId,
                Name = viewModel.QuizName,
                Questions = viewModel.Questions
            };

            _quizzes.Add(updatedQuiz);
            new DatabaseSync(_configuration).SyncChanges(updatedQuiz);

            return RedirectToAction("Details", new { Id = viewModel.QuizId });
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int quizId)
        {
            var quiz = _quizzes.Where(q => q.Id == quizId).FirstOrDefault();

            foreach (var question in quiz.Questions)
            {
                new DatabaseInteraction(_configuration).RemoveAnswer(new int[] { 0 }, question.Id);
            }
            new DatabaseInteraction(_configuration).RemoveQuestion(new int[] { 0 }, quizId);
            new DatabaseInteraction(_configuration).RemoveQuiz(quizId);

            _quizzes.Remove(quiz);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var vm = new CreateViewModel()
            {
                QuizName = "",
                Questions = new List<Question>()
            };
            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(CreateViewModel viewModel)
        {
            var newQuiz = new Quiz()
            {
                Name = viewModel.QuizName,
                Questions = viewModel.Questions
            };
            _quizzes.Add(newQuiz);
            new DatabaseSync(_configuration).SyncChanges(newQuiz);

            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult BlankQuestionRow()
        {
            var emptyQuestion = new Question()
            {
                Answers = new List<Answer>()
            };

            return PartialView("EditQuestionsRow", emptyQuestion);
        }

        public IActionResult BlankAnswerRow(string questionIdentifier)
        {
            ViewData["ContainerPrefix"] = questionIdentifier;
            return PartialView("EditAnswersRow", new Answer());
        }
    }
}
