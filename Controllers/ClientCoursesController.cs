using ComputerCourses.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ComputerCourses.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientCoursesController : ControllerBase
    {
        private readonly CourseContext _context;
        public ClientCoursesController(CourseContext context)
        {
            _context = context;
        }

        // GET: api/ClientCourse
        [HttpGet]
        [Authorize(Roles = "admin, teacher")]
        public async Task<ActionResult<IEnumerable<ClientCourse>>> GetClientCourses()
        {
            return await _context.ClientCourses.OrderBy(d => d.Id).ToListAsync();
        }

        //POST: api/ClientCourses
        //To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ClientCourse>> PostClientCourse(ClientCourse ClientCourse)
        {
            if (_context.ClientCourses == null)
            {
                return Problem("Entity set 'Context.ClientCourses' is null.");
            }

            var client = await _context.Clients.FindAsync(ClientCourse.ClientId);
            var course = await _context.Courses.FindAsync(ClientCourse.CourseId);

            if (course == null || client == null)
            {
                return NotFound();
            }

            _context.ClientCourses.Add(ClientCourse);
            
            await _context.SaveChangesAsync();

            return ClientCourse;
        }
        
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteClientCourse(int id)
        {
            var ClientCourse = await _context.ClientCourses.FindAsync(id);
            if (ClientCourse == null)
            {
                return NotFound();
            }

            _context.ClientCourses.Remove(ClientCourse);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
