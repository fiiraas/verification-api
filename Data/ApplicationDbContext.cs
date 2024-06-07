using Faver2.FaverModel;
using Microsoft.EntityFrameworkCore;

namespace Faver2.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options) : base(options)
        {
            
        }

        public DbSet<UserProfiles> Profiles { get; set; }
        public DbSet<ClockIn> ClockIns { get; set; }
    }
}
