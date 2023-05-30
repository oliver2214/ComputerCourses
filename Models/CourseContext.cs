using Microsoft.EntityFrameworkCore;

namespace ComputerCourses.Models
{
    public class CourseContext : DbContext
    {
        public CourseContext(DbContextOptions<CourseContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Course> Courses { get; set; } = null;
        public DbSet<Teacher> Teachers { get; set; } = null;
        public DbSet<Client> Clients { get; set; } = null;
        public DbSet<ClientCourse> ClientCourses { get; set; } = null;
    }

}