using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ComputerCourses.Models;
using Microsoft.AspNetCore.Authorization;

namespace ComputerCourses.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly CourseContext _context;

        public CoursesController(CourseContext context)
        {
            _context = context;
        }

        // GET: api/Courses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourses()
        {
            return await _context.Courses.ToListAsync();
        }

        // GET: api/Courses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Course>> GetCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);

            if (course == null)
            {
                return NotFound();
            }

            return course;
        }

        [HttpGet("GetCountClientsByCourse/{CourseId}")]
        public async Task<ActionResult<IEnumerable<(string CourseName, int StudentCount)>>> GetCountClientsByCourse(int CourseId)
        {
            if (!CourseExists(CourseId))
            {
                return NotFound();
            }
            var studentsCount = await _context.Descriptions
                                .Where(d => d.CourseId == CourseId)
                                .Join(_context.Courses,
                                    d => d.CourseId,
                                    c => c.Id,
                                    (d, c) => new { d, c })
                                .GroupBy(d => d.c.Title)
                                .Select(g => new { Title = g.Key, studentsCount = g.Count() })
                                .ToListAsync();
            if (studentsCount == null)
            {
                return NotFound();
            }
            return Ok(studentsCount);
        }


        [HttpGet("GetCoursesStartSoon")]
        public async Task<ActionResult<IEnumerable<Course>>> GetCoursesStartSoon()
        {
            var result = await _context.Courses
                                .Where(c => c.StudyStart <= DateTime.Now.AddMonths(3).ToUniversalTime() && c.StudyStart >= DateTime.Now.ToUniversalTime())
                                .ToListAsync();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet("GetTotalRevenueByCourses")]
        public async Task<ActionResult<IEnumerable<(string Title, int StudentCount, int TotalRevenue)>>> GetTotalRevenueByCourses()
        {
            var result = await _context.Courses
                        .Join(_context.Descriptions,
                                    c => c.Id,
                                    d => d.CourseId,
                                    (c, d) => new { course = c, description = d })
                        .GroupBy(c => new { c.course.Id, c.course.Title })
                        .Select(g => new {
                            Title = g.Key.Title,
                            StudentsCount = g.Count(),
                            TotalRevenue = g.Sum(c => c.course.Price)
                        })
                        .ToListAsync();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
        // PUT: api/Courses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "admin, teacher")]
        public async Task<IActionResult> PutCourse(int id, Course course)
        {
            if (id != course.Id)
            {
                return BadRequest();
            }

            _context.Entry(course).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Courses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "admin, teacher")]
        public async Task<ActionResult<Course>> PostCourse(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCourse", new { id = course.Id }, course);
        }

        // DELETE: api/Courses/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }

        
    }
}
