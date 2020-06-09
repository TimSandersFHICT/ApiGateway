using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KweetAPI.Model;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Kweet_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KweetController : ControllerBase
    {
        private readonly KweetContext _context;
        private ConnectionFactory factory;
        private IConnection conn;
        private IModel channel;

        public KweetController(KweetContext context)
        {
            _context = context;

            factory = new ConnectionFactory { HostName = "localhost" };
            conn = factory.CreateConnection();
            channel = conn.CreateModel();

            channel.QueueDeclare(
                queue: "UserQueue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
                );

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += async (model, eventArgs) =>
            {
                var body = eventArgs.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());

                Console.WriteLine($"[MessageQueue] Received message '{message}'");

                var messageParts = message.Split(":");

                var result = 0;
                if (messageParts[0] == "DELETE")
                {
                    result = await DeleteKweetsOfUserId(messageParts[1]);
                    Console.WriteLine($"[MessageQueue] Processed message '{message}' and deleted {result} records.");
                }
                if (messageParts[0] == "PUT")
                {
                    result = await UpdateKweetsOfUserId(messageParts[1], messageParts[2]);
                    Console.WriteLine($"[MessageQueue] Processed message '{message}' and updated {result} records.");
                }

               
            };

            channel.BasicConsume(queue: "UserQueue", autoAck: true, consumer: consumer);
        }

        //Destructor
        ~KweetController()
        {
            //Connection and Channels are meant to be long-lived
            //So we don't open and close them for each operation

            channel.Close();
            conn.Close();
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


        private async Task<int> DeleteKweetsOfUserId(string userId)
        {
            var deletedAmount = 0;
            var optionsBuilder = new DbContextOptionsBuilder<KweetContext>();
            optionsBuilder.UseInMemoryDatabase("KweetList");

            using (var tempContext = new KweetContext(optionsBuilder.Options))
            {
                var userKweets = await tempContext.KweetItems.Where(h => h.userID == userId).ToListAsync();
                deletedAmount = userKweets.Count;

                if (deletedAmount == 0) { return 0; }

                tempContext.KweetItems.RemoveRange(userKweets);
                await tempContext.SaveChangesAsync();
            }
            return deletedAmount;
        }

        private async Task<int> UpdateKweetsOfUserId(string userId, string username)
        {
            var updatedAmount = 0;
            var optionsBuilder = new DbContextOptionsBuilder<KweetContext>();
            optionsBuilder.UseInMemoryDatabase("KweetList");

            using (var tempContext = new KweetContext(optionsBuilder.Options))
            {
                var userKweets = await tempContext.KweetItems.Where(h => h.userID == userId).ToListAsync();

                foreach(Kweet kweet in userKweets)
                {
                    kweet.username = username;
                }
                updatedAmount = userKweets.Count;

                if (updatedAmount == 0) { return 0; }

                await tempContext.SaveChangesAsync();
            }
            return updatedAmount;
        }
        
    }
}