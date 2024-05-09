using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PFire.Infrastructure.Entities
{
    public class Friend : Entity
    {
        public uint MeId { get; set; }
        public User Me { get; set; }

        public uint ThemId { get; set; }
        public User Them { get; set; }

        public string Message { get; set; }

        public bool Pending { get; set; }
    }

    internal class FriendConfiguration : EntityConfiguration<Friend>
    {
        protected override void ConfigureEntity(EntityTypeBuilder<Friend> builder)
        {
            builder.ToTable(nameof(Friend));

            builder.HasKey(x => new
            {
                RequesterId = x.MeId,
                RecipientId = x.ThemId
            });

            builder.HasIndex(x => new
                   {
                       x.MeId,
                       x.ThemId
                   })
                   .IsUnique();

            builder.HasOne(x => x.Me)
                   .WithMany(x => x.MyFriends)
                   .HasForeignKey(x => x.MeId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Them)
                   .WithMany(x => x.FriendsOf)
                   .HasForeignKey(x => x.ThemId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Message).HasMaxLength(1000);
        }
    }
}
