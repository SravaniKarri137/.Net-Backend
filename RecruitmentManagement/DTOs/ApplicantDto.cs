namespace RecruitmentManagement.DTOs
{
    public class ApplicantDto
    {
        public int Id { get; set; }
        public string CandidateName { get; set; }
        public string Email { get; set; }
        public DateTime AppliedAt { get; set; }
        public string JobTitle { get; set; }
        public int JobPostingId { get; set; }

        public string ResumeUrl { get; set; }

        public string Status { get; set; }
        // Optional: show job title too
    }
}