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
            return await _context.Clients.OrderBy(c => c.Id).ToListAsync();
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
                        .Join(_context.ClientCourses,
                            client => client.Id,
                            ClientCourse => ClientCourse.ClientId,
                            (client, ClientCourse) => new { client, ClientCourse })
                        .Join(_context.Courses.Where(c => c.Id == CourseId),
                            cd => cd.ClientCourse.CourseId,
                            course => course.Id,
                            (cd, course) => new { cd.client, course })
                        .Select(d => new
                        {
                            Id = d.client.Id,
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
                        .Join(_context.ClientCourses,
                            client => client.Id,
                            ClientCourse => ClientCourse.ClientId,
                            (client, ClientCourse) => new { client, ClientCourse })
                        .Join(_context.Courses.Where(c => c.Id == CourseId),
                            cd => cd.ClientCourse.CourseId,
                            course => course.Id,
                            (cd, course) => new { cd, course })
                        .Select(d => new
                        {
                            Id = d.cd.client.Id,
                            Surname = d.cd.client.Surname,
                            Name = d.cd.client.Name,
                            Marks = d.cd.ClientCourse.Marks,
                            AverageMark = d.cd.ClientCourse.AverageMark(),
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
                        .Join(_context.ClientCourses,
                            client => client.Id,
                            ClientCourse => ClientCourse.ClientId,
                            (client, ClientCourse) => new { client, ClientCourse })
                        .Join(_context.Courses.Where(c => c.Id == CourseId),
                            cd => cd.ClientCourse.CourseId,
                            course => course.Id,
                            (cd, course) => new { cd, course })
                        .Select(d => new
                        {
                            Id = d.cd.client.Id,
                            Surname = d.cd.client.Surname,
                            Name = d.cd.client.Name,
                            Marks = d.cd.ClientCourse.Marks,
                            AverageMark = d.cd.ClientCourse.AverageMark(),
                        })
                        .ToListAsync();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet("GetClientTable/{ClientId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Client>>> GetClientTable(int ClientId)
        {
            var result = await _context.Clients.Where(c => c.Id == ClientId)
                        .Join(_context.ClientCourses,
                            client => client.Id,
                            ClientCourse => ClientCourse.ClientId,
                            (client, ClientCourse) => new { client, ClientCourse })
                        .Join(_context.Courses,
                            cd => cd.ClientCourse.CourseId,
                            course => course.Id,
                            (cd, course) => new { cd, course })
                        .Select(d => new
                        {
                            CourseId = d.course.Id,
                            CourseTitle = d.course.Title,
                            Marks = d.cd.ClientCourse.Marks,
                            AverageMark = d.cd.ClientCourse.AverageMark(),
                        })
                        .ToListAsync();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        // PUT: api/Clients/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<Client>> PutClient(int id, Client client)
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

            return Ok(client);
        }

        // POST: api/Clients
        [HttpPost]
        public async Task<ActionResult<Client>> PostClient(Client client)
        {
            bool usernameExists = await _context.Clients.AnyAsync(c => c.Username == client.Username) || await _context.Teachers.AnyAsync(c => c.Username == client.Username);
            if (usernameExists)
            {
                return BadRequest("This username is already exists");
            }

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostClient", new { id = client.Id }, client);
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
