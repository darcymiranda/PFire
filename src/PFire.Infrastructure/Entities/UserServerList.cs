using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PFire.Infrastructure.Entities
{
    public class UserServerList : Entity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int GameId { get; set; }
        public int GameIp { get; set; }
        public int GamePort { get; set; }
    }

    internal class UserServerListConfiguration : EntityConfiguration<UserServerList>
    {
        protected override void ConfigureEntity(EntityTypeBuilder<UserServerList> builder)
        {
            builder.ToTable(nameof(UserServerList));
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.HasIndex(x => x.Id);
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.GameId).IsRequired();
            builder.Property(x => x.GameIp).IsRequired();
            builder.Property(x => x.GamePort).IsRequired();
        }
    }
}
