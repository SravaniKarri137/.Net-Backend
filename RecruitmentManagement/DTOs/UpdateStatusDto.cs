namespace RecruitmentManagement.DTOs
{
    public class UpdateStatusDto
    {
        public string Status { get; set; }  // "Selected", "On Hold", "Rejected"
        public string Notes { get; set; }
        public int Rating { get; set; }

        // For scheduling interviews (only if Selected)
        public bool ScheduleInterview { get; set; }
        public DateTime InterviewDate { get; set; }
        public string Interviewer { get; set; }
        public string Mode { get; set; }  // "Online", "Offline", etc.
    }

}
