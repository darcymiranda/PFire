using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using PFire.Infrastructure.Entities;

namespace PFire.Infrastructure.Services
{
    internal interface IDatabaseContext
    {
        DbSet<T> Set<T>() where T : Entity;
        Task SaveChangesAsync();
    }

    internal class DatabaseContext : DbContext, IDatabaseContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) {}

        public DbSet<User> Users { get; set; }
        public DbSet<Friend> Friends { get; set; }

        DbSet<T> IDatabaseContext.Set<T>()
        {
            return Set<T>();
        }

        Task IDatabaseContext.SaveChangesAsync()
        {
            return SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var friendEntity = modelBuilder.Entity<Friend>();

            friendEntity.HasKey(x => new
            {
                RequesterId = x.MeId,
                RecipientId = x.ThemId
            });

            friendEntity.HasIndex(x => new
                        {
                            RequesterId = x.MeId,
                            RecipientId = x.ThemId
                        })
                        .IsUnique();

            friendEntity.HasOne(x => x.Me)
                        .WithMany(t => t.MyFriends)
                        .HasForeignKey(m => m.MeId)
                        .OnDelete(DeleteBehavior.Restrict);

            friendEntity.HasOne(x => x.Them)
                        .WithMany(t => t.FriendsOf)
                        .HasForeignKey(m => m.ThemId)
                        .OnDelete(DeleteBehavior.Restrict);
        }
    }

    internal class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
            optionsBuilder.UseSqlite("Data Source=blog.db");

            return new DatabaseContext(optionsBuilder.Options);
        }
    }
}
