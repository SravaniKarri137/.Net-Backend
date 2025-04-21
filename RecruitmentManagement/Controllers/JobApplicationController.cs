using Microsoft.AspNetCore.Mvc;

using RecruitmentManagement.Models;

using Microsoft.EntityFrameworkCore;

using RecruitmentManagement.Data;

using RecruitmentManagement.DTOs;
using RecruitmentManagement.Model;


namespace RecruitmentManagement.Controllers

{

    [Route("api/[controller]")]

    [ApiController]

    public class JobApplicationController : ControllerBase

    {

        private readonly AppDbContext _context;

        public JobApplicationController(AppDbContext context)

        {

            _context = context;

        }


        // ✅ POST: api/JobApplication/apply
        [HttpPost("apply")]
        public async Task<IActionResult> Apply(
            [FromForm] int jobPostingId,
            [FromForm] string candidateName,
            [FromForm] string email,
            [FromForm] IFormFile resume)
        {
            try
            {
                if (resume == null || resume.Length == 0)
                    return BadRequest(new { message = "Resume file is required." });

                Console.WriteLine("JobPostingId: " + jobPostingId);
                Console.WriteLine("CandidateName: " + candidateName);
                Console.WriteLine("Email: " + email);

                // Find the job posting
                var jobPosting = await _context.JobPostings.FindAsync(jobPostingId);

                if (jobPosting == null)
                    return BadRequest(new { message = "Job posting not found." });

                // Check if the candidate has applied in the last 3 months
                var existingApplication = await _context.JobApplications
                    .Where(a => a.JobPostingId == jobPostingId && a.Email == email)
                    .OrderByDescending(a => a.AppliedAt)
                    .FirstOrDefaultAsync();

                if (existingApplication != null)
                {
                    // Check if the last application was within the last 3 months
                    if (existingApplication.AppliedAt > DateTime.UtcNow.AddMonths(-3))
                    {
                        return BadRequest(new { message = "You have already applied for this job within the last 3 months." });
                    }
                }

                // Save the resume file
                var resumesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "resumes");

                if (!Directory.Exists(resumesFolder))
                    Directory.CreateDirectory(resumesFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(resume.FileName)}";
                var filePath = Path.Combine(resumesFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await resume.CopyToAsync(stream);
                }

                // Create a new application record
                var application = new JobApplication
                {
                    JobPostingId = jobPostingId,
                    CandidateName = candidateName,
                    Email = email,
                    AppliedAt = DateTime.UtcNow,
                    ResumeUrl = uniqueFileName
                };

                _context.JobApplications.Add(application);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Application submitted successfully!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR OCCURRED:");
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { message = "Server error: " + ex.Message });
            }
        }

    
       [HttpPost("update-status-and-shortlist/{id}")]
public async Task<IActionResult> UpdateStatusAndShortlist(int id, [FromBody] UpdateStatusDto dto)
{
    var application = await _context.JobApplications.FindAsync(id);
    if (application == null)
        return NotFound(new { message = "Application not found." });

    application.Status = dto.Status;
    await _context.SaveChangesAsync();

    var existingShortlist = await _context.ShortlistedProfiles
        .FirstOrDefaultAsync(s => s.JobApplicationId == id);

    if (dto.Status == "Selected")
    {
        if (existingShortlist != null)
        {
            existingShortlist.Notes = dto.Notes;
            existingShortlist.Rating = dto.Rating;
            existingShortlist.IsShortlisted = true;
        }
        else
        {
            var shortlisted = new ShortlistedProfile
            {
                JobApplicationId = id,
                Notes = dto.Notes,
                Rating = dto.Rating,
                IsShortlisted = true
            };
            _context.ShortlistedProfiles.Add(shortlisted);
        }

        if (dto.ScheduleInterview)
        {
            var existingInterview = await _context.Interviews
                .FirstOrDefaultAsync(i => i.JobApplicationId == id);

            if (existingInterview == null)
            {
                var interview = new Interview
                {
                    JobApplicationId = id,
                    ScheduledAt = dto.InterviewDate,
                    Interviewer = dto.Interviewer,
                    Mode = dto.Mode,
                    Status = InterviewStatus.Scheduled
                };
                _context.Interviews.Add(interview);
            }
        }

        await _context.SaveChangesAsync();
    }
    else if (dto.Status == "On Hold")
    {
        if (existingShortlist != null)
        {
            existingShortlist.Notes = dto.Notes;
            existingShortlist.Rating = dto.Rating;
            existingShortlist.IsShortlisted = false;
        }
        else
        {
            var onHoldEntry = new ShortlistedProfile
            {
                JobApplicationId = id,
                Notes = dto.Notes,
                Rating = dto.Rating,
                IsShortlisted = false
            };
            _context.ShortlistedProfiles.Add(onHoldEntry);
        }

        await _context.SaveChangesAsync();
    }

    // Rejected: only status is updated — nothing to do
    return Ok(new { message = $"Status updated to {dto.Status}" });
}




        // ✅ GET: api/JobApplication/by-job/{jobId}

        [HttpGet("by-job/{jobId}")]

        public async Task<ActionResult<IEnumerable<ApplicantDto>>> GetApplicationsByJob(int jobId)

        {

            var job = await _context.JobPostings.FindAsync(jobId);

            if (job == null)

                return NotFound("Job not found");

            var applications = await _context.JobApplications

                .Where(a => a.JobPostingId == jobId)

                .Select(a => new ApplicantDto

                {
                    Id = a.Id,
                    CandidateName = a.CandidateName,

                    Email = a.Email,
                    
                    JobPostingId = a.JobPostingId,

                    AppliedAt = a.AppliedAt,

                    JobTitle = job.Title,


                    ResumeUrl = $"{Request.Scheme}://{Request.Host}/resumes/{a.ResumeUrl}",
                    Status = a.Status

                })

                .ToListAsync();

            return Ok(applications);

        }
        [HttpGet("applications/by-email")]
        public async Task<ActionResult<IEnumerable<ApplicantDto>>> GetApplicationsByEmail([FromQuery] string email)
        {
            var applications = await _context.JobApplications
                .Where(a => a.Email.ToLower() == email.ToLower())
                .Include(a => a.JobPosting)
                .Select(a => new ApplicantDto
                {
                    Id = a.Id,
                    CandidateName = a.CandidateName,
                    Email = a.Email,
                    JobPostingId = a.JobPostingId,
                    AppliedAt = a.AppliedAt,
                    JobTitle = a.JobPosting.Title,
                    ResumeUrl = $"{Request.Scheme}://{Request.Host}/resumes/{a.ResumeUrl}",
                    Status = a.Status
                })
                .ToListAsync();

            if (!applications.Any())
                return NotFound("No applications found for the given email.");

            return Ok(applications);
        }




    }

}

