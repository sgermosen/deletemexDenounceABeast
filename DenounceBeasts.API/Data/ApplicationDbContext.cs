
using DenounceBeasts.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DenounceBeasts.API.Data
{
    public class ApplicationDbContext: Microsoft.EntityFrameworkCore.DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Municipality> Municipalities { get; set; }
        public DbSet<Sector> Sectors { get; set; }
    }
}
