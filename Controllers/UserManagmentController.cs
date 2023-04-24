using ComputerCourses.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Specialized;
using System.Security.Claims;

namespace ComputerCourses.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagmentController : ControllerBase
    {
        private readonly CourseContext _context;
        public UserManagmentController(CourseContext context)
        {
            _context = context;
        }
        public struct LoginData
        {
            public string login { get; set; }
            public string password { get; set; }
        }

        [HttpPost("Authorization")]
        public object GetClientToken([FromBody] LoginData ld)
        {
            var user = _context.Clients.FirstOrDefault(u => u.Username == ld.login && u.Password == ld.password);
            var teacher = _context.Teachers.FirstOrDefault(u => u.Username == ld.login && u.Password == ld.password);
            if (user == null && teacher == null)
            {
                Response.StatusCode = 401;
                return new { message = "wrong login/password" };
            }

            var role = user != null ? user.Role : teacher.Role;
            if (user != null)
                {
                return new { token = AuthOptions.GenerateToken(user.Role), message = user.WelcomeMessage() };
                }
            else
            {
                return new { token = AuthOptions.GenerateToken(teacher.Role), message = teacher.WelcomeMessage() };
            }
        }


        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }

        private bool TeacherExists(int id)
        {
            return _context.Teachers.Any(e => e.Id == id);
        }

        [HttpPut("ChangeUsername/{id}")]
        [Authorize]
        public async Task<ActionResult<object>> ChangeUsername(int id, [FromForm] string new_username)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (role == "guest")
            {
                var client = await _context.Clients.FindAsync(id);
                if (client == null)
                {
                    return NotFound();
                }
                var usernameExists = await _context.Clients.AnyAsync(c => c.Username == new_username);
                if (usernameExists)
                {
                    return BadRequest("Username already exists");
                }
                client.Username = new_username;
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return client;
            }

            if (role == "teacher")
            {
                var teacher = await _context.Teachers.FindAsync(id);
                if (teacher == null)
                {
                    return NotFound();
                }
                var usernameExists = await _context.Teachers.AnyAsync(c => c.Username == new_username);
                if (usernameExists)
                {
                    return BadRequest("Username already exists");
                }
                teacher.Username = new_username;
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return teacher;
            }
            else { return BadRequest("Unknown Authorization role"); }
        }

        [HttpPut("ChangePassword/{id}")]
        [Authorize]
        public async Task<ActionResult<object>> ChangePassword(int id, [FromForm] string new_password)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (role == "guest")
            {
                var client = await _context.Clients.FindAsync(id);
                if (client == null)
                {
                    return NotFound();
                }
                client.Password = new_password;
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return client;
            }

            if (role == "teacher")
            {
                var teacher = await _context.Teachers.FindAsync(id);
                if (teacher == null)
                {
                    return NotFound();
                }
                teacher.Password = new_password;
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return teacher;
            }
            else { return BadRequest("Unknown Authorization role"); }
        }
    }
}
