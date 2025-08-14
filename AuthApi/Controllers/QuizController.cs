using AuthApi.Data;
using AuthApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AuthApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class QuizController: ControllerBase
    {
        private readonly AppDbContext _db;
        public QuizController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("random")]
      [Authorize]

        public IActionResult GetRandomQuestion() {

            var question = _db.Questions
                .Include(q => q.Answers)
                .OrderBy(r => Guid.NewGuid())
                .FirstOrDefault();

            if (question == null)
            {
                return NotFound("Nie znaleziono pytań w bazie danych.");
            }
            else
            {
                return Ok(new
                {
                    question.Id,
                    question.QuestionText,
                    Answers = question.Answers.Select(a => new { a.Id, a.AnswerText })
                });

            }
        }

        [HttpPost("{id}/check")]
        [Authorize]
        public IActionResult CheckAnswer(int id, [FromBody] int answerId) 
        {
            var answer = _db.Answers.FirstOrDefault(a => a.Id == answerId && a.QuestionId == id);

            if (answer == null)
            {
                return NotFound("Nie znaleziono odpowiedzi.");
            }
            else
            {
                return Ok(new { correct = answer.IsCorrect });

            }

        }

        
    }
}
