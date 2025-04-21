
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RecruitmentManagement.Models;

namespace RecruitmentManagement.Model
{
    public class Interview
    {
        [Key]
        public int Id { get; set; }

        // Foreign key to JobApplication
        public int JobApplicationId { get; set; }

        [ForeignKey("JobApplicationId")]
        public JobApplication JobApplication { get; set; }

        // Date and time scheduled
        public DateTime ScheduledAt { get; set; }

        // Interviewer's name or email
        public string Interviewer { get; set; }

        // Notes after the interview
        public string ?InterviewNotes { get; set; }

        // Feedback after interview
        public string ?Feedback { get; set; }

        // Interview mode (e.g., Online, Offline, Phone)
        public string Mode { get; set; }

        // Interview status: Scheduled, Completed, Cancelled
        public InterviewStatus Status { get; set; } = InterviewStatus.Scheduled;
    }

    public enum InterviewStatus
    {
        Scheduled,
        Completed,
        Cancelled
    }


}
