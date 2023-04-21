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
        public async Task<ActionResult<IEnumerable<Description>>> GetDescriptions()
        {
            return await _context.Descriptions.ToListAsync();
        }

        // POST: api/Descriptions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Description>> PostDescription(Description Description)
        {
            _context.Descriptions.Add(Description);
            await _context.SaveChangesAsync();

            return Description;
        }
    }
}
