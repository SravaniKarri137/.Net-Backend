// Models/JobApplication.cs
using System;

namespace RecruitmentManagement.Models
{
    public class JobApplication
    {
        public int Id { get; set; }
        public int JobPostingId { get; set; }
        //public JobPosting JobPosting { get; set; }
        public JobPosting? JobPosting { get; set; }

        public string CandidateName { get; set; }
        public string Email { get; set; }
        public string ResumeUrl { get; set; }
        //public DateTime AppliedAt { get; set; }
        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

        public string Status { get; set; } = "Applied";
    }
}
