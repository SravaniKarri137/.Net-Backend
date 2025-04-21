using Microsoft.AspNetCore.Mvc;
using RecruitmentManagement.Data;
using RecruitmentManagement.Model;
using Microsoft.EntityFrameworkCore;

namespace RecruitmentManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/Auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
            {
                return BadRequest("Username already exists.");
            }

            // For now storing plain password (we’ll hash it later)
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok("Registration successful");
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User login)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Username == login.Username && u.Password == login.Password);

            if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            return Ok(new
            {
                Message = "Login successful",
                Role = user.Role,
                UserId = user.UserId,
                Email = user.Email,
                Username= user.Username
            });
        }

    }
}
