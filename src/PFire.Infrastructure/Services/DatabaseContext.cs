using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PFire.Infrastructure.Entities;

namespace PFire.Infrastructure.Services
{
    public interface IDatabaseMigrator
    {
        Task Migrate();
    }

    public interface IDatabaseContext
    {
        DbSet<T> Set<T>() where T : Entity;
        Task SaveChanges();
    }

    internal class DatabaseContext : DbContext, IDatabaseContext, IDatabaseMigrator
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) {}

        DbSet<T> IDatabaseContext.Set<T>()
        {
            return Set<T>();
        }

        Task IDatabaseContext.SaveChanges()
        {
            return SaveChangesAsync();
        }

        public Task Migrate()
        {
            return Database.MigrateAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new FriendConfiguration());
            modelBuilder.ApplyConfiguration(new GroupConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }
    }
}
