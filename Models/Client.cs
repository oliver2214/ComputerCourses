﻿using NuGet.Protocol.Plugins;

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
        public List<ClientCourse>? ClientCourses { get; set; }

    }
}
