using Microsoft.Extensions.Configuration;
using QuizManager.Models;
using System.Linq;

namespace QuizManager.Controllers.Methods
{
    public class DatabaseSync
    {
        private readonly IConfiguration _configuration;

        public DatabaseSync(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SyncChanges(Quiz quiz)
        {
            SyncQuiz(quiz);
        }

        private void SyncQuiz(Quiz quiz)
        {
            int quizId;
            if (quiz.Id == 0)
            {
                quizId = new DatabaseInteraction(_configuration).InsertQuiz(quiz);
                quiz.Id = quizId;
            }
            else
            {
                quizId = quiz.Id;
                new DatabaseInteraction(_configuration).UpdateQuiz(quiz);
            }

            foreach (var question in quiz.Questions)
            {
                question.QuizId = quizId;
                SyncQuestions(question);
            }

            new DatabaseInteraction(_configuration).RemoveQuestion(quiz.Questions.Select(a => a.Id).ToArray(), quizId);
        }

        private void SyncQuestions(Question question)
        {
            int questionId;
            if (question.Id == 0)
            {
                questionId = new DatabaseInteraction(_configuration).InsertQuestion(question);
                question.Id = questionId;
            }
            else
            {
                questionId = question.Id;
                new DatabaseInteraction(_configuration).UpdateQuestion(question);
            }

            foreach (var answer in question.Answers)
            {
                answer.QuestionId = questionId;
                SyncAnswers(answer);
            }
            new DatabaseInteraction(_configuration).RemoveAnswer(question.Answers.Select(a => a.Id).ToArray(), questionId);
        }

        private void SyncAnswers(Answer answer)
        {
            int answerId;
            if (answer.Id == 0)
            {
                answerId = new DatabaseInteraction(_configuration).InsertAnswer(answer);
                answer.Id = answerId;
            }
            else
            {
                new DatabaseInteraction(_configuration).UpdateAnswer(answer);
            }
        }
    }
}
