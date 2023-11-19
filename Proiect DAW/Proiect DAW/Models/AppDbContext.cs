using Microsoft.EntityFrameworkCore;

namespace Proiect_DAW.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }

        //public DbSet<[MODEL]> [MODELS] { get; set; }

        //
    }
}
