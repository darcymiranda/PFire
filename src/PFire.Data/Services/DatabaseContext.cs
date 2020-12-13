using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PFire.Data.Entities;

namespace PFire.Data.Services
{
    public interface IDatabaseContext
    {
        DbSet<T> Set<T>() where T : Entity;
        Task SaveChanges();
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

        Task IDatabaseContext.SaveChanges()
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
                        .WithMany(x => x.MyFriends)
                        .HasForeignKey(x => x.MeId)
                        .OnDelete(DeleteBehavior.Restrict);

            friendEntity.HasOne(x => x.Them)
                        .WithMany(x => x.FriendsOf)
                        .HasForeignKey(x => x.ThemId)
                        .OnDelete(DeleteBehavior.Restrict);

            var userEntity = modelBuilder.Entity<User>();

            userEntity.HasKey(x => x.Id);
            userEntity.Property(x => x.Id).ValueGeneratedOnAdd();
            userEntity.Property(x => x.Username).IsRequired();
            userEntity.Property(x => x.Password).IsRequired();
            userEntity.Property(x => x.Salt).IsRequired();
        }
    }
}
