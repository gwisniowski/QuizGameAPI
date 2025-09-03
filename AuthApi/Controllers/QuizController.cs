using AuthApi.Data;
using AuthApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace AuthApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class QuizController : ControllerBase
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


        [HttpPost("question")]

        public async Task<IActionResult> AddQuestion([FromBody] AddQuestion dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var isAdmin = await _db.Admins.AnyAsync(a => a.UserId == userId);

            if (!isAdmin)
                return Forbid();



            var isExisting = await _db.Questions.AnyAsync(q => q.QuestionText == dto.QuestionText);

            if (isExisting)
                return BadRequest(new { message = "Pytanie już istnieje" });

            var question = new Question
            {
                QuestionText = dto.QuestionText,
                Answers = dto.Answers.Select(a => new Answer
                {
                    AnswerText = a.AnswerText,
                    IsCorrect = a.IsCorrect
                }).ToList()
            };

            _db.Questions.Add(question);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Dodano pytanie" });


        }











    }
}
