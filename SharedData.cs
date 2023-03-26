using System.Collections.Generic;
using ComputerCourses.Models;

namespace ComputerCourses
{
    public static class SharedData
    {
        public static HashSet<string> Summaries { get; } = new HashSet<string>
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public static List<Client> Clients { get; } = new List<Client>
        {
            new Client(){ Username = "user", Password = "user" },
            new Client(){ Username = "admin", Password = "admin" },
        };
    }
}
