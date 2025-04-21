// Models/JobPosting.cs
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;

namespace RecruitmentManagement.Models
{
    public class JobPosting
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public double Salary { get; set; }
        public string Department { get; set; }
        public string JobType { get; set; }
        public DateTime Deadline { get; set; }
        public JobStatus Status { get; set; }
        [JsonIgnore]
        [NotMapped]
        public ICollection<JobApplication>? JobApplications
        { get; set; }
    
    }

    public enum JobStatus
        {
            Published,
            Unpublished
        }
    
}

