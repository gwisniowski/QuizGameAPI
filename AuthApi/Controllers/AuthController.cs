
using AuthApi.Data;
using AuthApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;


namespace AuthApi.Controllers
{

   


    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        
        private readonly AppDbContext _context;

        
        public AuthController( AppDbContext context)
        {
            _context = context;
        }
      
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Wszystkie pola są wymagane.");

            if (dto.Password != dto.ConfirmPassword)
                return BadRequest("Hasła nie są zgodne.");

            var existingUser = await _context.UserProfiles.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (existingUser != null)
                return BadRequest("Użytkownik o tym adresie e-mail już istnieje.");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new UserProfile
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PasswordHash = hashedPassword
              
            };

            _context.UserProfiles.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Rejestracja zakończona sukcesem!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Email i hasło są wymagane.");
            var user = await _context.UserProfiles.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Nieprawidłowy e-mail lub hasło.");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("TwojSuperTajnySekretnyKluczJwt123!");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                  {
                     new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                     new Claim(ClaimTypes.Email, user.Email)
                  }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);


            return Ok(new { token = jwt, message = "Zalogowano pomyślnie!" });
        }

        
        [HttpGet("profile")]
        [Authorize]


        public async Task<IActionResult> GetProfile() 
        {

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(); 
            }
            var userId = userIdClaim.Value;

            var user = await _context.UserProfiles.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
           


            if (user == null)
            {
                return NotFound();

            }
            else 
            {
                var isAdmin = await _context.Admins.AnyAsync(a => a.UserId == user.Id);
               
                return Ok(new 
                { 
                    user.Email, 
                    user.UserName,
                    user.CompletedQuizes,
                    user.LastResult,
                    user.CreatedAt,
                    user.AvatarIndex,
                    isAdmin,

                 
                });

            }


        }

        [Authorize]
        [HttpPut("update-result")]


        public async Task<IActionResult> UpdateResult([FromBody] UpdateResultDto dto)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized("Brak ID użytkownika w tokenie");


            var user = await _context.UserProfiles.FirstOrDefaultAsync(u => u.Id == int.Parse(userId));
            if (user == null)
                return NotFound("Nie znaleziono profilu użytkownika");

            user.LastResult = dto.LastResult;
            user.CompletedQuizes += 1;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Wynik zaktualizowany",
                lastResult = user.LastResult,
                completedQuizes = user.CompletedQuizes
            });



        }

        [Authorize]
        [HttpPatch("avatar")]

        public async Task<IActionResult> ChangeAvatar([FromBody] int avatarIndex) 
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized("Brak ID użytkownika w tokenie");

            var user = await _context.UserProfiles.FirstOrDefaultAsync(u => u.Id == int.Parse(userId));
            if (user == null)
                return NotFound("Nie znaleziono profilu użytkownika");

            user.AvatarIndex = avatarIndex;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Avatar updated", avatarIndex = user.AvatarIndex });

        }



    }
}
