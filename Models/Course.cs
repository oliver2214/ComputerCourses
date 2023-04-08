using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ComputerCourses.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string? Technologies { get; set; }
        public int? StudyDuration { get; set; }
        public int? TeachersId { get; set; }
        public int ClientId { get; set; }      // внешний ключ
        public Client Client { get; set; }    // навигационное свойство
    }
}
