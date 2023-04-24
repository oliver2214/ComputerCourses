using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ComputerCourses.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ComputerCourses.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly CourseContext _context;
        public ClientsController(CourseContext context)
        {
            _context = context;
        }

        // GET: api/Clients
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<Client>>> GetClients()
        {
            return await _context.Clients.ToListAsync();
        }

        // GET: api/Clients/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Client>> GetClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);

            if (client == null)
            {
                return NotFound();
            }

            return client;
        }


        [HttpGet("GetClientsByCourse/{CourseId}")]
        [Authorize(Roles = "admin, teacher")]
        public async Task<ActionResult<IEnumerable<Client>>> GetClientsByCourse(int CourseId)
        {
            var result = await _context.Clients
                        .Join(_context.Descriptions,
                            client => client.Id,
                            description => description.ClientId,
                            (client, description) => new { client, description })
                        .Join(_context.Courses.Where(c => c.Id == CourseId),
                            cd => cd.description.CourseId,
                            course => course.Id,
                            (cd, course) => new { cd.client, course })
                        .Select(d => new
                        {
                            Surname = d.client.Surname,
                            Name = d.client.Name,
                        })
                        .ToListAsync();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet("GetClientMarksByCourse/{ClientId}/{CourseId}")]
        [Authorize]
        public async Task<ActionResult<Client>> GetClientMarksByCourse(int ClientId, int CourseId)
        {
            var result = await _context.Clients.Where(c => c.Id == ClientId)
                        .Join(_context.Descriptions,
                            client => client.Id,
                            description => description.ClientId,
                            (client, description) => new { client, description })
                        .Join(_context.Courses.Where(c => c.Id == CourseId),
                            cd => cd.description.CourseId,
                            course => course.Id,
                            (cd, course) => new { cd, course })
                        .Select(d => new
                        {
                            Surname = d.cd.client.Surname,
                            Name = d.cd.client.Name,
                            Marks = d.cd.description.Marks,
                            AverageMark = d.cd.description.AverageMark(),
                        })
                        .FirstOrDefaultAsync();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet("GetClientsMarksByCourse/{CourseId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Client>>> GetClientsMarksByCourse(int CourseId)
        {
            var result = await _context.Clients
                        .Join(_context.Descriptions,
                            client => client.Id,
                            description => description.ClientId,
                            (client, description) => new { client, description })
                        .Join(_context.Courses.Where(c => c.Id == CourseId),
                            cd => cd.description.CourseId,
                            course => course.Id,
                            (cd, course) => new { cd, course })
                        .Select(d => new
                        {
                            Surname = d.cd.client.Surname,
                            Name = d.cd.client.Name,
                            Marks = d.cd.description.Marks,
                            AverageMark = d.cd.description.AverageMark(),
                        })
                        .ToListAsync();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        // PUT: api/Clients/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutClient(int id, Client client)
        {
            if (id != client.Id)
            {
                return BadRequest();
            }

            _context.Entry(client).State = EntityState.Modified;

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

            return NoContent();
        }

        // POST: api/Clients
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Client>> PostClient(Client client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetClient", new { id = client.Id }, client);
        }

        // DELETE: api/Clients/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }

        
    }
}
