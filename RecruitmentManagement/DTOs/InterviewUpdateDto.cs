namespace RecruitmentManagement.Model
{
    public class InterviewUpdateDTO
    {
        public int Id { get; set; }
        public int JobApplicationId { get; set; }
        public DateTime ScheduledAt { get; set; }
        public string Interviewer { get; set; }
        public string? InterviewNotes { get; set; }
        public string? Feedback { get; set; }
        public string Mode { get; set; }
        public InterviewStatus Status { get; set; }
    }
}
