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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // Seed Data

            modelBuilder.Entity<Semesters>().
                HasData(
                new Semesters { Id = 1, Name = "1-1" },
                new Semesters { Id = 2, Name = "1-2" },
                new Semesters { Id = 3, Name = "2-1" },
                new Semesters { Id = 4, Name = "2-2" },
                new Semesters { Id = 5, Name = "3-1" },
                new Semesters { Id = 6, Name = "3-2" },
                new Semesters { Id = 7, Name = "4-1" },
                new Semesters { Id = 8, Name = "4-2" }

             );


        }

    }
}
