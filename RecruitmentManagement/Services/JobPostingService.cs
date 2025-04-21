// Services/JobPostingService.cs
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RecruitmentManagement.Data;
using RecruitmentManagement.Models;


namespace RecruitmentManagement.Services
{
    public class JobPostingService
    {
        private readonly AppDbContext _context;

        public JobPostingService(AppDbContext context)
        {
            _context = context;
        }

        // Get all job postings
        public async Task<List<JobPosting>> GetAllJobPostingsAsync()
        {
            return await _context.JobPostings.ToListAsync();
        }

        // Get a single job posting by Id
        public async Task<JobPosting> GetJobPostingByIdAsync(int id)
        {
            return await _context.JobPostings.FindAsync(id);
        }
        public async Task<IEnumerable<JobPosting>> GetJobPostingsByDepartmentAsync(string department)
        {
            return await _context.JobPostings
                                 .Where(j => j.Department == department)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<JobPosting>> GetJobPostingsByFilterAsync(string jobType, string location, JobStatus? status, string department)
        {
            var query = _context.JobPostings.AsQueryable();

            if (!string.IsNullOrWhiteSpace(jobType))
                query = query.Where(j => j.JobType.ToLower() == jobType.ToLower());

            if (!string.IsNullOrWhiteSpace(location))
                query = query.Where(j => j.Location.ToLower() == location.ToLower());

            if (status.HasValue)
                query = query.Where(j => j.Status == status);

            if (!string.IsNullOrWhiteSpace(department))
                query = query.Where(j => j.Department.ToLower() == department.ToLower());

            return await query.ToListAsync();
        }






        // Create a new job posting
        public async Task<JobPosting> CreateJobPostingAsync(JobPosting jobPosting)
        {
            _context.JobPostings.Add(jobPosting);
            await _context.SaveChangesAsync();
            return jobPosting;
        }

        // Update an existing job posting
        public async Task<JobPosting> UpdateJobPostingAsync(int id, JobPosting jobPosting)
        {
            var existingJobPosting = await _context.JobPostings.FindAsync(id);
            if (existingJobPosting != null)
            {
                existingJobPosting.Title = jobPosting.Title;
                existingJobPosting.Description = jobPosting.Description;
                existingJobPosting.Location = jobPosting.Location;
                existingJobPosting.Salary = jobPosting.Salary;
                existingJobPosting.Department = jobPosting.Department;
                existingJobPosting.JobType = jobPosting.JobType;
                existingJobPosting.Deadline = jobPosting.Deadline;
                existingJobPosting.Status = jobPosting.Status;

                await _context.SaveChangesAsync();
            }
            return existingJobPosting;
        }

        // Delete a job posting
        public async Task<bool> DeleteJobPostingAsync(int id)
        {
            var jobPosting = await _context.JobPostings.FindAsync(id);
            if (jobPosting != null)
            {
                _context.JobPostings.Remove(jobPosting);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
