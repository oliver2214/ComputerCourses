namespace ComputerCourses.Models
{
    public class Person
    {
        public Person(string Email, string Password) 
        {
            Email = this.Email;
            Password = this.Password;

        }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
