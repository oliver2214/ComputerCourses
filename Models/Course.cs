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
    }
}
