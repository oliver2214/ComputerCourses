using ComputerCourses.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpPost("Client")]
        public object GetClientToken([FromBody] LoginData ld)
        {
            var user = _context.Clients.FirstOrDefault(u => u.Username == ld.login && u.Password == ld.password);
            if (user == null)
            {
                Response.StatusCode = 401;
                return new { message = "wrong login/password" };
            }
            return AuthOptions.GenerateToken(user.IsAdmin);
        }

        [HttpPost("Teacher")]
        public object GetTeacherToken([FromBody] LoginData ld)
        {
            var user = _context.Teachers.FirstOrDefault(u => u.Username == ld.login && u.Password == ld.password);
            if (user == null)
            {
                Response.StatusCode = 401;
                return new { message = "wrong login/password" };
            }
            return AuthOptions.GenerateToken(user.IsAdmin);
        }

        [HttpGet("jwttoken")]
        public object GetToken()
        {
            return AuthOptions.GenerateToken();
        }

        [HttpGet("jwttoken/admin")]
        public object GetAdminToken()
        {
            return AuthOptions.GenerateToken(true);
        }

        [HttpGet("Clients")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<Client>>> GetClients()
        {
            return await _context.Clients.ToListAsync();
        }
    }
}
