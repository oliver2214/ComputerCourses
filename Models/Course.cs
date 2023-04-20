using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ComputerCourses.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public List<string>? Technologies { get; set; }
        public int StudyDuration { get; set; }
        public List<Description>? Descriptions { get; set; }
    }
}
