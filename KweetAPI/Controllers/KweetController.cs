using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KweetAPI.Model;

namespace Kweet_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KweetController : ControllerBase
    {
        private readonly KweetContext _context;

        public KweetController(KweetContext context)
        {
            _context = context;
        }

        // GET: api/Kweet
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Kweet>>> GetKweetItems()
        {
            return await _context.KweetItems.ToListAsync();
        }

        // GET: api/Kweet/5
        [HttpGet("{id}")]
        public async Task<ActionResult<List<Kweet>>> GetKweet(int id)
        {
            var Kweet = _context.GetByUserid(id);

            if (Kweet == null)
            {
                return NotFound();
            }

            return Kweet;
        }

        // PUT: api/Kweet/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutKweet(int id, Kweet Kweet)
        {
            if (id != Kweet.id)
            {
                return BadRequest();
            }

            _context.Entry(Kweet).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KweetExists(id))
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

        // POST: api/Kweet
        [HttpPost]
        public async Task<ActionResult<Kweet>> PostKweet(Kweet Kweet)
        {
            Kweet.created = DateTime.Now;
            _context.KweetItems.Add(Kweet);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetKweet", new { id = Kweet.id }, Kweet);
        }

        // DELETE: api/Kweet/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Kweet>> DeleteKweet(int id)
        {
            var Kweet = await _context.KweetItems.FindAsync(id);
            if (Kweet == null)
            {
                return NotFound();
            }

            _context.KweetItems.Remove(Kweet);
            await _context.SaveChangesAsync();

            return Kweet;
        }

        private bool KweetExists(long id)
        {
            return _context.KweetItems.Any(e => e.id == id);
        }
    }
}