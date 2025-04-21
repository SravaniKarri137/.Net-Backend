using RecruitmentManagement.Model;
using Microsoft.EntityFrameworkCore;
using RecruitmentManagement.Models;

namespace RecruitmentManagement.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<JobPosting> JobPostings { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<ShortlistedProfile> ShortlistedProfiles { get; set; }
        public DbSet<Interview> Interviews { get; set; }


        // Configuring unique constraint for Email using Fluent API
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ensure Email is unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
            modelBuilder.Entity<Interview>()
                .Property(i => i.Status)
                .HasConversion<string>(); // converts enum to string in DB

            base.OnModelCreating(modelBuilder);
        }
        

    }
}
