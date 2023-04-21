﻿using System;
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

        //GET: api/Clients/5
        [HttpGet("/get2/{id}")]
        public async Task<ActionResult<Course>> GetCourse2(int id)
        {
            var Course = await _context.Courses
                                .Where(c => c.Id == id)
                                .FirstOrDefaultAsync();

            if (Course == null)
            {
                return NotFound();
            }

            return Course;
        }

        [HttpGet("/test1")]
        public async Task<ActionResult<IEnumerable<Client>>> Gettest1()
        {
            var clients = await _context.Clients
                                .Select(c => new { c.Name, c.Surname })
                                .ToListAsync();
            return Ok(clients);
        }

        [HttpGet("/test2")]
        public async Task<ActionResult<IEnumerable<Client>>> Gettest2()
        {
            var clients = await _context.Clients
                                .Where(c => c.Role == "guest")
                                .Select(c => new { c.Name, c.Surname, c.Role })
                                .ToListAsync();
            return Ok(clients);
        }

        [HttpGet("/test3")]
        public async Task<ActionResult<IEnumerable<Client>>> Gettest3()
        {
            var clients = await _context.Clients
                                .Where(c => c.Role == "guest")
                                .Select(c => new { c.Name, c.Surname, c.Role })
                                .OrderBy(p => p.Name)
                                .ToListAsync();
            return Ok(clients);
        }

        [HttpGet("/test4")]
        public async Task<ActionResult<IEnumerable<Client>>> Gettest4()
        {
            var result = await _context.Clients
                        .Join(
                            _context.Descriptions,
                            client => client.Id,
                            description => description.ClientId,
                            (client, description) => new { client, description })
                        .Join(_context.Courses,
                            cd => cd.description.CourseId,
                            course => course.Id,
                            (cd, course) => new {
                                Surname = cd.client.Surname,
                                Name = cd.client.Name,
                                Title = course.Title,
                                AvgMarks = cd.description.AverageMark()
                            })
                        .OrderBy(c => c.Title)
                        .ToListAsync();
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

        [HttpPut("/ChangeUsername/{id}/{new_username}")]
        [Authorize]
        public async Task<ActionResult<object>> ChangeUsername(int id, string new_username)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (role == "guest")
            {
                var client = await _context.Clients.FindAsync(id);
                if (client == null)
                {
                    return NotFound();
                }
                var usernameExists = await _context.Clients.AnyAsync(c => c.Username == new_username);
                if (usernameExists)
                {
                    return BadRequest("Username already exists");
                }
                client.Username = new_username;
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
                return client;
            }

            if (role == "teacher")
            {
                var teacher = await _context.Teachers.FindAsync(id);
                if (teacher == null)
                {
                    return NotFound();
                }
                var usernameExists = await _context.Teachers.AnyAsync(c => c.Username == new_username);
                if (usernameExists)
                {
                    return BadRequest("Username already exists");
                }
                teacher.Username = new_username;
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
                return teacher;
            }
            else { return BadRequest("Unknown Authorization role"); }
        }

        [HttpPut("/ChangePassword/{id}/{new_Password}")]
        [Authorize]
        public async Task<ActionResult<object>> ChangePassword(int id, string new_password)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (role == "guest")
            {
                var client = await _context.Clients.FindAsync(id);
                if (client == null)
                {
                    return NotFound();
                }
                client.Password = new_password;
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
                return client;
            }

            if (role == "teacher")
            {
                var teacher = await _context.Teachers.FindAsync(id);
                if (teacher == null)
                {
                    return NotFound();
                }
                teacher.Password = new_password;
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
                return teacher;
            }
            else { return BadRequest("Unknown Authorization role"); }
        }
    }
}
