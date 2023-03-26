using ComputerCourses.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Specialized;

namespace ComputerCourses.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly CourseContext _context;
        public AuthController(CourseContext context)
        {
            _context = context;
        }
        public struct LoginData
        {
            public string login { get; set; }
            public string password { get; set; }
        }

        [HttpPost]
        public object GetToken([FromBody] LoginData ld)
        {
            var user = _context.Clients.FirstOrDefault(u => u.Username == ld.login && u.Password == ld.password);
            if (user == null)
            {
                Response.StatusCode = 401;
                return new { message = "wrong login/password" };
            }
            return AuthOptions.GenerateToken(user.IsAdmin);
        }
    }
}
