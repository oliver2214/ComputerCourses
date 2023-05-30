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

            var existingClientCourse = await _context.ClientCourses.FirstOrDefaultAsync(cc =>
                cc.ClientId == ClientCourse.ClientId && cc.CourseId == ClientCourse.CourseId);

            if (existingClientCourse != null)
            {
                return Conflict("The combination of ClientId and CourseId already exists.");
            }

            var studentsCount = await _context.ClientCourses
                .Where(d => d.CourseId == ClientCourse.CourseId)
                .Include(d => d.Course)
                .GroupBy(d => d.Course.Id)
                .Select(g => new { StudentCount = g.Count() })
                .ToListAsync();

            if (course.MaxStudents <= studentsCount.Count)
            {
                return Conflict($"Достигнуто максимальное число учеников на курсе: {course.MaxStudents}");
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
