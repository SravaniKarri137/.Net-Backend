using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitmentManagement.Data;
using RecruitmentManagement.Model;

namespace RecruitmentManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProfileController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Profile>> GetProfile(int id)
        {
            var profile = await _context.Profiles
                .Include(p => p.User) // Optional: Include User info if needed
                .FirstOrDefaultAsync(p => p.ProfileId == id);

            if (profile == null)
            {
                return NotFound();
            }

            return profile;
        }
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<Profile>> GetProfileByUserId(int userId)
        {
            var profile = await _context.Profiles
                .Include(p => p.User) // Optional: Include User info if needed
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
            {
                return NotFound();
            }

            return profile;
        }


        // PUT: api/Profile/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProfile(int id, Profile profile)
        {
            if (id != profile.ProfileId)
            {
                return BadRequest();
            }

            _context.Entry(profile).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProfileExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Profile

        [HttpPost]
        public async Task<ActionResult<Profile>> PostProfile(Profile profile)
        {
            // Check if a profile already exists for the given UserId
            var existingProfile = await _context.Profiles
                .FirstOrDefaultAsync(p => p.UserId == profile.UserId);

            if (existingProfile != null)
            {
                return BadRequest("❌ A profile with this UserId already exists. Cannot add another.");
            }

            _context.Profiles.Add(profile);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProfile), new { id = profile.ProfileId }, profile);
        }


        // DELETE: api/Profile/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProfile(int id)
        {
            var profile = await _context.Profiles.FindAsync(id);
            if (profile == null)
            {
                return NotFound();
            }

            _context.Profiles.Remove(profile);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProfileExists(int id)
        {
            return _context.Profiles.Any(e => e.ProfileId == id);
        }
    }
}
