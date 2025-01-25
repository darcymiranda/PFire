using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PFire.Infrastructure.Entities
{
    public class Group : Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int OwnerId { get; set; }
        public User Owner { get; set; }
    }
    
    internal class GroupConfiguration : EntityConfiguration<Group>
    {
        protected override void ConfigureEntity(EntityTypeBuilder<Group> builder)
        {
            //builder.ToTable(nameof(Group));

            //builder.HasKey(x => x.Id);
            //builder.Property(x => x.Id).ValueGeneratedOnAdd();
            //builder.HasIndex(x => x.Id);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(1000);
        }
    }
}
