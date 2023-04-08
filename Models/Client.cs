using NuGet.Protocol.Plugins;

namespace ComputerCourses.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int? CourseId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public List<Course> Courses { get; set; }
        public bool IsAdmin => Role == "admin";
    }
}
