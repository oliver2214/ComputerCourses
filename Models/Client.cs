using NuGet.Protocol.Plugins;

namespace ComputerCourses.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }    
        public List<Description>? Descriptions { get; set; }


        public string WelcomeMessage()
        {
            if (Role == "admin")
                return "Добро пожаловать. Все права доступны в полной мере!";
            else
                return $"Добро пожаловать {Name} {Surname}!";
        }

        public void ChangeUsername(string new_username)
        {
            Username = new_username;
        }

        public void ChangePassword(string new_password)
        {
            Password = new_password;
        }

    }
}
