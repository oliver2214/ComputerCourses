using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ComputerCourses.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public List<string> Technologies { get; set; } = new List<string>();
        public int StudyDuration { get; set; }
        public int MaxStudents { get; set; }
        public DateTime StudyStart { get; set; } = DateTime.Now.ToUniversalTime();
        public List<ClientCourse>? ClientCourses { get; set; } = new List<ClientCourse>();
        public List<Teacher>? Teachers { get; set; } = new List<Teacher>();

    }
}
