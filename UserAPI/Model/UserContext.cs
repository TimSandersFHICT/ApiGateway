using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace UserAPI.Model
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
        }

        public DbSet<User> UserItems { get; set; }

        public List<User> GetByUserid(int userId)
        {
            return UserItems.Where(r => userId.Equals(r.id)).ToList();
        }
    }

}