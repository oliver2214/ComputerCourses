namespace ComputerCourses.Models
{
    public class Teacher
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime? EmploymentDate { get; set; } = DateTime.Now;
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public List<Course>? Courses { get; set; }

        public string WelcomeMessage()
        {
            if (Role == "admin")
                return "Добро пожаловать. Все права доступны в полной мере!";
            else
                return $"Добро пожаловать преподаватель {Name} {Surname}!";
        }
    }
}
