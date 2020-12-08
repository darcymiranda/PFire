using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PFire.Data.Entities;

namespace PFire.Data.Services
{
    internal interface IDatabaseContext
    {
        DbSet<T> Set<T>() where T : Entity;
        Task SaveChanges();
        Task<IDbContextTransaction> BeginTransaction();
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

        public Task<IDbContextTransaction> BeginTransaction()
        {
            return Database.BeginTransactionAsync();
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
}
