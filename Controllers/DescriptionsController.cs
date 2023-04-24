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
    public class DescriptionsController : ControllerBase
    {
        private readonly CourseContext _context;
        public DescriptionsController(CourseContext context)
        {
            _context = context;
        }

        

        // GET: api/Descriptions
        [HttpGet]
        [Authorize(Roles = "admin, teacher")]
        public async Task<ActionResult<IEnumerable<Description>>> GetDescriptions()
        {
            return await _context.Descriptions.OrderBy(d => d.Id).ToListAsync();
        }

        

        //POST: api/Descriptions
        //To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Description>> PostDescription(Description Description)
        {
            _context.Descriptions.Add(Description);
            await _context.SaveChangesAsync();

            return Description;
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteDescription(int id)
        {
            var description = await _context.Descriptions.FindAsync(id);
            if (description == null)
            {
                return NotFound();
            }

            _context.Descriptions.Remove(description);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
