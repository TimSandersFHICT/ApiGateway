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
             //SeedContextOnStartup();
             
        }

        public virtual DbSet<Kweet> KweetItems { get; set; }

        public List<Kweet> GetByUserid(int userId)
        {
            return KweetItems.Where(r => userId.Equals(r.id)).ToList();
        }

        private void SeedContextOnStartup()
        {
             var kweet1 = new Kweet
            {
                id = 0,
                text = "Dit is een kweet",
            };
            
            KweetItems.Add(kweet1);
            this.SaveChanges();
        }
    }

}