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
    public class CourseTeacherController : ControllerBase
    {
        private readonly CourseContext _context;
        public CourseTeacherController(CourseContext context)
        {
            _context = context;
        }


        // Put: api/CourseTeacher/AddCourseTeacher
        [HttpPut("AddCourseTeacher")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> PutCourseTeacher([FromForm] int courseID, [FromForm] int teacherID)
        {
            var course = await _context.Courses.Include(c => c.Teachers).FirstOrDefaultAsync(c => c.Id == courseID);
            var teacher = await _context.Teachers.Include(t => t.Courses).FirstOrDefaultAsync(t => t.Id == teacherID);

            if (course == null || teacher == null)
            {
                return NotFound();
            }

            var existingCourseTeacher = course.Teachers.FirstOrDefault(t => t.Id == teacherID);

            if (existingCourseTeacher != null)
            {
                return Conflict("Преподаватель уже ведет этот курс.");
            }

            course.Teachers.Add(teacher);
            teacher.Courses.Add(course);
            await _context.SaveChangesAsync();

            return Ok("Связь между курсом и преподавателем успешно добавлена");
        }

        // Put: api/CourseTeacher/DeleteCourseTeacher
        [HttpPut("DeleteCourseTeacher")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteCourseTeacher([FromForm] int courseID, [FromForm] int teacherID)
        {
            var course = await _context.Courses.Include(c => c.Teachers).FirstOrDefaultAsync(c => c.Id == courseID);
            var teacher = await _context.Teachers.Include(t => t.Courses).FirstOrDefaultAsync(t => t.Id == teacherID);

            if (course == null || teacher == null)
            {
                return NotFound();
            }

            var existingCourseTeacher = course.Teachers.FirstOrDefault(t => t.Id == teacherID);

            course.Teachers.Remove(teacher);
            teacher.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return Ok("Связь между курсом и преподавателем успешно разорвана");
        }
    }
}
