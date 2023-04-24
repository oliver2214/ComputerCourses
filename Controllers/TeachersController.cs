using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ComputerCourses.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace ComputerCourses.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachersController : ControllerBase
    {
        private readonly CourseContext _context;

        public TeachersController(CourseContext context)
        {
            _context = context;
        }

        // GET: api/Teachers
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<Teacher>>> GetTeachers()
        {
            return await _context.Teachers.ToListAsync();
        }

        // GET: api/Teachers/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin, teacher")]
        public async Task<ActionResult<Teacher>> GetTeacher(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);

            if (teacher == null)
            {
                return NotFound();
            }

            return teacher;
        }

        [HttpGet("GetCoursesByTeacher/{TeacherId}")]
        public async Task<ActionResult<IEnumerable<Course>>> GetCoursesByTeacher(int TeacherId)
        {
            if (!TeacherExists(TeacherId))
            {
                return NotFound();
            }
            var result = await _context.Teachers
                                    .Where(t => t.Id == TeacherId)
                                    .SelectMany(c => c.Courses)
                                    .Select(g => new { g.Title })
                                    .ToListAsync();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        

        // PUT: api/Teachers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "admin, teacher")]
        public async Task<IActionResult> PutTeacher(int id, Teacher teacher)
        {
            if (id != teacher.Id)
            {
                return BadRequest();
            }

            _context.Entry(teacher).State = EntityState.Modified;

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

            return NoContent();
        }

        [HttpPut("AddMarkForStudent/{courseId}/{studentId}")]
        public IActionResult AddMarkForStudent(int courseId, int studentId, [FromForm] int mark)
        {
            // Находим запись Description для указанного курса и студента
            var description = _context.Descriptions.FirstOrDefault(d => d.CourseId == courseId && d.ClientId == studentId);

            // Если запись не найдена, возвращаем 404 ошибку
            if (description == null)
            {
                return NotFound();
            }

            // Обновляем список оценок
            description.Marks.Add(mark);

            // Сохраняем изменения
            _context.SaveChanges();

            // Возвращаем успешный результат
            return Ok();
        }

        // POST: api/Teachers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Teacher>> PostTeacher(Teacher teacher)
        {
            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTeacher", new { id = teacher.Id }, teacher);
        }

        // DELETE: api/Teachers/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin, teacher")]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }

            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TeacherExists(int id)
        {
            return _context.Teachers.Any(e => e.Id == id);
        }
    }
}
