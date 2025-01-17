using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PFire.Infrastructure.Entities
{
    public class UserGroup : Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OwnerId { get; set; }
        public List<int> MemberIds { get; set; }
    }

    internal class UserGroupConfiguration : EntityConfiguration<UserGroup>
    {
        protected override void ConfigureEntity(EntityTypeBuilder<UserGroup> builder)
        {
            builder.ToTable(nameof(UserGroup));
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.HasIndex(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(1000);
            builder.HasIndex(x => x.Name);
            builder.Property(x => x.OwnerId).IsRequired();
        }
    }
}
