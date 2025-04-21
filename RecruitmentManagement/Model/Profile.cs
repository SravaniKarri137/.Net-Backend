// Profile.cs (formerly Candidate.cs)
using System.ComponentModel.DataAnnotations.Schema;

namespace RecruitmentManagement.Model
{
    public class Profile
    {
        public int ProfileId { get; set; } // Primary Key (renamed from CandidateId)
        public int UserId { get; set; }    // Foreign Key to User

        public string FullName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Skills { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public User? User { get; set; } // Navigation property
    }
}
