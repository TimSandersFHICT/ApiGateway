using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserAPI.Model;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace UserAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly IConfiguration _configuration;
        private ConnectionFactory factory;
        private IConnection conn;
        private IModel channel;


        public UserController(UserContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;

            var RabbitMQOption = _configuration.GetSection(RabbitMQOptions.Position)
                .Get<RabbitMQOptions>();

            factory = new ConnectionFactory { HostName = RabbitMQOption.Connection };
            conn = factory.CreateConnection();
            channel = conn.CreateModel();

            channel.QueueDeclare(
                queue: "UserQueue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
                );
        }

        //Destructor
        ~UserController()
        {
            //Connection and Channels are meant to be long-lived
            //So we don't open and close them for each operation

            channel.Close();
            conn.Close();
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            LogData();

            return await _context.UserItems
                .Include(user => user)
                .ToListAsync();
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            //reason not to use the default "FindAsync" method:
            //https://stackoverflow.com/a/39095357

            var user = await _context.UserItems
                .Include(user => user)
                .FirstOrDefaultAsync(user => user.id == id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/User/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(string id, User user)
        {
            if (id != user.id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                string message = $"PUT:{user.id}:{user.username}";
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                    routingKey: "UserQueue",
                                    basicProperties: null,
                                    body: body);

                System.Console.WriteLine($"[MessageQueue] Produced message '{message}'");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.UserItems.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.id }, user);
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(string id)
        {
            var user = await _context.UserItems.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.UserItems.Remove(user);
            await _context.SaveChangesAsync();

            string message = $"DELETE:{id}";
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "",
                                routingKey: "UserQueue",
                                basicProperties: null,
                                body: body);

            System.Console.WriteLine($"[MessageQueue] Produced message '{message}'");

            return user;
        }

        private void LogData()
        {
            System.Diagnostics.Debug.WriteLine("Help");
        }

        private bool UserExists(string id)
        {
            return _context.UserItems.Any(e => e.id == id);
        }
    }
}
