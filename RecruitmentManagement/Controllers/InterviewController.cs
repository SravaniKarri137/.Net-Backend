using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitmentManagement.Data;
using RecruitmentManagement.DTOs;
using System.Threading.Tasks;
using System.Linq;
using RecruitmentManagement.Model;

namespace RecruitmentManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InterviewController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InterviewController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Interview/by-application/5
        [HttpGet("by-application/{jobAppId}")]
        public async Task<IActionResult> GetInterviewByApplicationId(int jobAppId)
        {
            var interview = await _context.Interviews
                .Where(i => i.JobApplicationId == jobAppId)
                .Select(i => new InterviewResponseDto
                {
                    Id = i.Id,
                    JobApplicationId = i.JobApplicationId,
                    ScheduledAt = i.ScheduledAt,
                    Interviewer = i.Interviewer,
                    Mode = i.Mode,
                    Status = i.Status.ToString(), // convert enum to string
                    InterviewNotes = i.InterviewNotes,
                    Feedback = i.Feedback
                })
                .FirstOrDefaultAsync();

            if (interview == null)
                return NotFound(new { message = "Interview not found for this application." });

            return Ok(interview);
        }

        // GET: api/Interview/all
        [HttpGet("all")]
        public async Task<IActionResult> GetAllInterviews()
        {
            var interviews = await _context.Interviews
                .Select(i => new InterviewResponseDto
                {
                    Id = i.Id,
                    JobApplicationId = i.JobApplicationId,
                    ScheduledAt = i.ScheduledAt,
                    Interviewer = i.Interviewer,
                    Mode = i.Mode,
                    Status = i.Status.ToString(),
                    InterviewNotes = i.InterviewNotes,
                    Feedback = i.Feedback // Convert enum to string
                })
                .ToListAsync();

            if (interviews == null || !interviews.Any())
            {
                return NotFound(new { message = "No interviews found." });
            }

            return Ok(interviews);
        }

       
        // PUT: api/Interview/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInterview(int id, [FromBody] InterviewUpdateDTO updatedInterviewDTO)
        {
            // Check if the provided interview id matches the id in the URL
            if (id != updatedInterviewDTO.Id)
            {
                return BadRequest(new { message = "Interview ID mismatch" });
            }

            // Find the interview to update
            var interview = await _context.Interviews.FindAsync(id);
            if (interview == null)
            {
                return NotFound(new { message = "Interview not found" });
            }

            // Map the DTO values to the interview entity
            interview.ScheduledAt = updatedInterviewDTO.ScheduledAt;
            interview.Interviewer = updatedInterviewDTO.Interviewer;
            interview.Mode = updatedInterviewDTO.Mode;
            interview.Status = updatedInterviewDTO.Status;
            interview.InterviewNotes = updatedInterviewDTO.InterviewNotes;
            interview.Feedback = updatedInterviewDTO.Feedback;

            try
            {
                // Save the changes to the database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InterviewExists(id))
                {
                    return NotFound(new { message = "Interview not found during update" });
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { message = "Interview updated successfully", interview });
        }

        // Helper function to check if the interview exists
        private bool InterviewExists(int id)
        {
            return _context.Interviews.Any(e => e.Id == id);
        }


    }
}
