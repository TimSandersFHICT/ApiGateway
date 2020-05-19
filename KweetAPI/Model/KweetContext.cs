using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace KweetAPI.Model
{
    public class KweetContext : DbContext
    {
        public KweetContext(DbContextOptions<KweetContext> options)
            : base(options)
        {
        }

        public DbSet<Kweet> KweetItems { get; set; }

        public List<Kweet> GetByUserid(int userId)
        {
            return KweetItems.Where(r => userId.Equals(r.id)).ToList();
        }
    }

}