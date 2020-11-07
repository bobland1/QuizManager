using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using QuizManager.Models;
using System.Collections.Generic;
using System.Linq;
using Dapper;

namespace QuizManager.Controllers.Methods
{
    public class DatabaseInteraction
    {
        private readonly IConfiguration _configuration;

        private readonly SqlConnection Connection;
        public DatabaseInteraction(IConfiguration configuration)
        {
            _configuration = configuration;
            Connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }

        public List<Quiz> FetchQuizzesFromDatabase()
        {
            using (Connection)
            {
                var quizzes = Connection.Query<Quiz>("Select * From Quizzes").ToList();
                foreach (var quiz in quizzes)
                {
                    quiz.Questions = Connection.Query<Question>("Select * From Questions Where QuizId = @id", quiz).ToList();

                    foreach (var question in quiz.Questions)
                    {
                        question.Answers = Connection.Query<Answer>("Select * From Answers Where QuestionId = @id", question).ToList();
                    }
                }

                return quizzes;
            }
        }

        public int InsertQuiz(Quiz quiz)
        {
            using (Connection)
            {
                return Connection.QuerySingle<int>("INSERT INTO Quizzes(Name) VALUES(@Name) SELECT SCOPE_IDENTITY()", quiz);
            }
        }

        public void UpdateQuiz(Quiz quiz)
        {
            using (Connection)
            {
                Connection.Execute("UPDATE Quizzes Set Name = @Name WHERE ID = @Id", quiz);
            }
        }

        public void RemoveQuiz(int quizId)
        {
            using (Connection)
            {
                Connection.Execute("DELETE FROM Quizzes WHERE ID = @quizId", new { quizId });
            }
        }

        public int InsertQuestion(Question question)
        {
            using (Connection)
            {
                return Connection.QuerySingle<int>("INSERT INTO Questions(Text, QuizId) VALUES(@Text, @QuizId) SELECT SCOPE_IDENTITY()", question);
            }
        }

        public void UpdateQuestion(Question question)
        {
            using (Connection)
            {
                Connection.Execute("UPDATE Questions Set Text = @Text WHERE ID = @Id", question);
            }
        }

        public void RemoveQuestion(int[] questionIds, int quizId)
        {
            using (Connection)
            {
                Connection.Execute("DELETE FROM Questions WHERE ID NOT IN @questionIds AND QuizId = @QuizId", new { questionIds, quizId });
            }
        }

        public int InsertAnswer(Answer answer)
        {
            using (Connection)
            {
                return Connection.QuerySingle<int>("INSERT INTO Answers(Text, IsCorrect, QuestionId) VALUES(@Text, @IsCorrect, @QuestionId) SELECT SCOPE_IDENTITY()", answer);
            }
        }

        public void UpdateAnswer(Answer answer)
        {
            using (Connection)
            {
                Connection.Execute("UPDATE Answers Set Text = @Text, IsCorrect = @IsCorrect WHERE ID = @Id", answer);
            }
        }

        public void RemoveAnswer(int[] answerIds, int questionId)
        {
            using (Connection)
            {
                Connection.Execute("DELETE FROM Answers WHERE ID NOT IN @answerIds AND QuestionId = @QuestionId", new { answerIds, questionId });
            }
        }
    }
}
