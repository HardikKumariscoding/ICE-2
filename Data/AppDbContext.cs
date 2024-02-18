using Microsoft.EntityFrameworkCore;
using WebApplication22.Models;

namespace WebApplication22.Data
{
    public class AppDbContext :DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)

        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=(Localdb)\\mssqllocaldb;Database=ProjectManagementDb5;Trusted_Connection=True;MultipleActiveResultSets=True",
                options => options.EnableRetryOnFailure());
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectTask> ProjectTasks { get; set; }

    }
}
