using Kolekcje.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Kolekcje.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {

        public DbSet<Collection> Collections { get; set; }
        public DbSet<CollectionCategory> CollectionCategories { get; set; }
        public IEnumerable<object>? Categories { get; internal set; }
    }

}
