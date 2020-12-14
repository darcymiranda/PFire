using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PFire.Data.Entities;

namespace PFire.Data.Services
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

        public DbSet<User> Users { get; set; }
        public DbSet<Friend> Friends { get; set; }

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
            var friendEntity = modelBuilder.Entity<Friend>();

            friendEntity.HasKey(x => new
            {
                RequesterId = x.MeId,
                RecipientId = x.ThemId
            });

            friendEntity.HasIndex(x => new
                        {
                            x.MeId,
                            x.ThemId
                        })
                        .IsUnique();

            friendEntity.HasOne(x => x.Me)
                        .WithMany(x => x.MyFriends)
                        .HasForeignKey(x => x.MeId)
                        .OnDelete(DeleteBehavior.Restrict);

            friendEntity.HasOne(x => x.Them)
                        .WithMany(x => x.FriendsOf)
                        .HasForeignKey(x => x.ThemId)
                        .OnDelete(DeleteBehavior.Restrict);

            friendEntity.Property(x => x.Message).HasMaxLength(1000);

            var userEntity = modelBuilder.Entity<User>();

            userEntity.HasKey(x => x.Id);
            userEntity.Property(x => x.Id).ValueGeneratedOnAdd();
            userEntity.HasIndex(x => x.Id);
            userEntity.Property(x => x.Username).IsRequired().HasMaxLength(1000);
            userEntity.HasIndex(x => x.Username);
            userEntity.Property(x => x.Password).IsRequired();
            userEntity.Property(x => x.Salt).IsRequired();
            userEntity.Property(x => x.Nickname).HasMaxLength(1000);
        }
    }
}
