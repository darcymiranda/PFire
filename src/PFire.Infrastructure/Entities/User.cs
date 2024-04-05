using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PFire.Infrastructure.Entities
{
    public class User : Entity
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string Nickname { get; set; }
        public List<Friend> MyFriends { get; set; }
        public List<Friend> FriendsOf { get; set; }
        public bool ShowGameStatusToFriends { get; set; }
        public bool ShowGameServerData { get; set; }
        public bool ShowGameDataOnProfile { get; set; }
        public bool ShowTimeStampInChat { get; set; }
        public bool ShowVoiceChatServer { get; set; }
        public bool ShowTyping { get; set; }
        public bool ShowFriendsOfFriends { get; set; }
        public bool PlaySoundOnNewMessages { get; set; }
        public bool PlaySoundsOnNewMessagesInGame { get; set; }
        public bool PlaySoundsOnLogOn { get; set; }
        public bool ShowOfflineFriends { get; set; }
        public bool ShowNicknames { get; set; }
        public bool ShowTooltipOnLogOn { get; set; }
        public bool ShowTooltipOnDownload { get; set; }
        public bool PlaySoundInChatrooms { get; set; }
        public bool PlaySoundOnVoicecalls { get; set; }
        public bool PlaySoundOnScreenshots { get; set; }
    }

    internal class UserConfiguration : EntityConfiguration<User>
    {
        protected override void ConfigureEntity(EntityTypeBuilder<User> builder)
        {
            builder.ToTable(nameof(User));
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.HasIndex(x => x.Id);
            builder.Property(x => x.Username).IsRequired().HasMaxLength(1000);
            builder.HasIndex(x => x.Username);
            builder.Property(x => x.Password).IsRequired();
            builder.Property(x => x.Salt).IsRequired();
            builder.Property(x => x.Nickname).HasMaxLength(1000);
        }
    }
}
