using Microsoft.EntityFrameworkCore;
using prototype_2_login_backend.Models;

namespace prototype_2_login_backend.Context
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity <User>().ToTable("users");
        }
    }
}
