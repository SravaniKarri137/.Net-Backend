using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RecruitmentManagement.Models;
namespace RecruitmentManagement.Model
{
   
    

    public class ShortlistedProfile
    {
        [Key]
        public int Id { get; set; }

        // Foreign key to JobApplication
        public int JobApplicationId { get; set; }

        [ForeignKey("JobApplicationId")]
        public JobApplication JobApplication { get; set; }

        // Admin's notes about the candidate
        public string Notes { get; set; }

        // Admin rating (e.g., 1 to 5)
        [Range(1, 5)]
        public int Rating { get; set; }

        // Whether the candidate is officially shortlisted
        public bool IsShortlisted { get; set; } = false;
    }

}
