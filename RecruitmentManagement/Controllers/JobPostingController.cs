// Controllers/JobPostingController.cs
using Microsoft.AspNetCore.Mvc;
using RecruitmentManagement.Models;
using RecruitmentManagement.Services;


namespace RecruitmentManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobPostingController : ControllerBase
    {
        private readonly JobPostingService _jobPostingService;

        public JobPostingController(JobPostingService jobPostingService)
        {
            _jobPostingService = jobPostingService;
        }


        // GET: api/jobposting
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobPosting>>> GetAllJobPostings()
        {
            var jobPostings = await _jobPostingService.GetAllJobPostingsAsync();
            return Ok(jobPostings);
        }

        // GET: api/jobposting/5
        [HttpGet("{id}")]
        public async Task<ActionResult<JobPosting>> GetJobPostingById(int id)
        {
            var jobPosting = await _jobPostingService.GetJobPostingByIdAsync(id);
            if (jobPosting == null)
            {
                return NotFound();
            }
            return Ok(jobPosting);
        }

        [HttpGet("by-department")]
        public async Task<ActionResult<IEnumerable<JobPosting>>> GetJobPostingsByDepartment([FromQuery] string department)
        {
            if (string.IsNullOrEmpty(department))
            {
                return BadRequest("Department query parameter is required.");
            }

            try
            {
                department = department.Trim();  // Remove any unwanted newlines/spaces

                var jobPostings = await _jobPostingService.GetJobPostingsByDepartmentAsync(department);

                if (jobPostings == null || !jobPostings.Any())
                {
                    return NotFound($"No job postings found for department: {department}");
                }

                return Ok(jobPostings);
            }
            catch (Exception ex)
            {
                // Log the exception and return a server error response
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("by-job-type-location-status-department")]
        public async Task<ActionResult<IEnumerable<JobPosting>>> GetJobPostingsByFilter(
    [FromQuery] string? jobType,
    [FromQuery] string? location,
    [FromQuery] string? status,
    [FromQuery] string? department)
        {
            try
            {
                // Ensure at least one filter is provided
                if (string.IsNullOrWhiteSpace(jobType) &&
                    string.IsNullOrWhiteSpace(location) &&
                    string.IsNullOrWhiteSpace(status) &&
                    string.IsNullOrWhiteSpace(department))
                {
                    return BadRequest("You must provide at least one filter: jobType, location, status, or department.");
                }

                JobStatus? jobStatus = null;

                // Parse the status if provided
                if (!string.IsNullOrWhiteSpace(status))
                {
                    if (Enum.TryParse(status, true, out JobStatus parsedStatus))
                    {
                        jobStatus = parsedStatus;
                    }
                    else
                    {
                        return BadRequest($"Invalid status value: {status}");
                    }
                }

                // Fetch job postings based on filters, even if one or more are null
                var jobPostings = await _jobPostingService.GetJobPostingsByFilterAsync(jobType, location, jobStatus, department);

                if (jobPostings == null || !jobPostings.Any())
                {
                    return NotFound("No job postings found with the specified filters.");
                }

                return Ok(jobPostings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }








        // POST: api/jobposting
        [HttpPost]
        public async Task<ActionResult<JobPosting>> CreateJobPosting([FromBody] JobPosting jobPosting)
        {
            var newJobPosting = await _jobPostingService.CreateJobPostingAsync(jobPosting);
            return CreatedAtAction(nameof(GetJobPostingById), new { id = newJobPosting.Id }, newJobPosting);
        }


        // PUT: api/jobposting/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJobPosting(int id, JobPosting jobPosting)
        {
            var updatedJobPosting = await _jobPostingService.UpdateJobPostingAsync(id, jobPosting);
            if (updatedJobPosting == null)
            {
                return NotFound();
            }
            return NoContent();
        }

        // DELETE: api/jobposting/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJobPosting(int id)
        {
            var deleted = await _jobPostingService.DeleteJobPostingAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
