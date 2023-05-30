using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ComputerCourses.Models;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;

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
            return await _context.Courses.OrderBy(c => c.Id).ToListAsync();
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
        [Authorize]
        public async Task<ActionResult<IEnumerable<(int CourseId, string CourseName, int StudentCount)>>> GetCountClientsByCourse(int CourseId)
        {
            if (!CourseExists(CourseId))
            {
                return NotFound();
            }

            var studentsCount = await _context.ClientCourses
                .Where(d => d.CourseId == CourseId)
                .Include(d => d.Course)
                .GroupBy(d => new { d.Course.Id, d.Course.Title } )
                .Select(g => new { CourseId = g.Key.Id, CourseName = g.Key.Title, StudentCount = g.Count() })
                .ToListAsync();

            if (studentsCount == null)
            {
                return NotFound();
            }
            return Ok(studentsCount);
        }


        [HttpGet("GetCountClientsByCourses")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<(int CourseId, int StudentCount)>>> GetCountClientsByCourses()
        {
            var studentsCount = await _context.ClientCourses
                                .Join(_context.Courses,
                                    d => d.CourseId,
                                    c => c.Id,
                                    (d, c) => new { d, c })
                                .GroupBy(d => new { d.d.CourseId, d.c.Title } )
                                .Select(g => new { CourseId = g.Key.CourseId, CourseName = g.Key.Title, studentsCount = g.Count() })
                                .OrderBy(n => n.CourseId)
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
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<(string Title, int StudentCount, int TotalRevenue)>>> GetTotalRevenueByCourses()
        {
            var result = await _context.Courses
                        .Join(_context.ClientCourses,
                                    c => c.Id,
                                    d => d.CourseId,
                                    (c, d) => new { course = c, ClientCourse = d })
                        .GroupBy(c => new { c.course.Id, c.course.Title })
                        .Select(g => new {
                            CourseId = g.Key.Id,
                            Title = g.Key.Title,
                            StudentsCount = g.Count(),
                            TotalRevenue = g.Sum(c => c.course.Price)
                        })
                        .OrderBy(c => c.CourseId)
                        .ToListAsync();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet("GetTeachersByCourse/{courseId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetTeachersByCourses(int courseId)
        {
            var teachers = await _context.Courses
                .Where(c => c.Id == courseId)
                .SelectMany(c => c.Teachers)
                .Select(g => new { TeacherId = g.Id, Name = g.Name, Surname = g.Surname })
                .ToListAsync();

            if (teachers.Count == 0)
            {
                return Ok("На курсе нет преподавателей.");
            }

            return teachers;
        }

        // PUT: api/Courses/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin, teacher")]
        public async Task<ActionResult<Course>> PutCourse(int id, Course course)
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

            return course;
        }

        // POST: api/Courses
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<Course>> PostCourse(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCourse", new { id = course.Id }, course);
        }

        // DELETE: api/Courses/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin, teacher")]
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
