using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentResultManagement.Models;

namespace StudentResultManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

       

        public DbSet<Series> Series { get; set; }
        public DbSet<Students> Students { get; set; }

        public DbSet<Semesters> Semesters { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<Result> Results { get; set; }


    }
}
