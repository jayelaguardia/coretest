using Microsoft.EntityFrameworkCore;
using coretest.Domain.Models;

namespace coretest.Persistence.Contexts
{
    public class CoreTestDbContext : DbContext
    {
        public DbSet<User> User { get; set; }

        public CoreTestDbContext(DbContextOptions<CoreTestDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().ToTable("User");
            builder.Entity<User>().HasKey(p => p.id);
            builder.Entity<User>().Property(p => p.id).IsRequired().ValueGeneratedOnAdd();
            builder.Entity<User>().Property(p => p.username).IsRequired().HasMaxLength(30);
            builder.Entity<User>().Property(p => p.password).IsRequired().HasMaxLength(72);

            builder.Entity<User>().HasData
            (
                new User { id = 1, username = "dunder", password = "mifflin" }
            );
        }
    }
}