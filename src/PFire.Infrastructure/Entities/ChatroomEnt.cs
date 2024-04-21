using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PFire.Infrastructure.Entities
{
    public class ChatroomEnt : Entity
    {
        public int Id { get; set; }
        public byte[] Cid { get; set; } = new byte[21];
        public string Name { get; set; } = "";
        public int Visibility { get; set; } = 1;
        public int DefaultPerms { get; set; } = 2;
        public List<int> Users { get; set; } = new List<int>();
        public List<int> PowerUsers { get; set; } = new List<int>();
        public List<int> Moderators { get; set; } = new List<int>();
        public List<int> Administrators { get; set; } = new List<int>();
        public List<int> SilencedUsers { get; set; } = new List<int>();
        public byte ShowJoinLeaveMessages { get; set; } = 1;
        public byte SavedRoom { get; set; } = 0;
        public byte Silenced { get; set; } = 0;
        public string MOTD { get; set; } = "";
        public string Password { get; set; } = "";
        public int GameLobbyHost { get; set; } = 0;
        public int GameLobbyID { get; set; } = 0;
        public int GameLobbyIP { get; set; } = 0;
        public int GameLobbyPort { get; set; } = 0;
        public List <int> GameLobbyPlayers { get; set;} = new List<int>();
    }
    internal class ChatroomConfiguration : EntityConfiguration<ChatroomEnt>
    {
        protected override void ConfigureEntity(EntityTypeBuilder<ChatroomEnt> builder)
        {
            builder.ToTable(nameof(ChatroomEnt));
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.HasIndex(x => x.Id);
        }
    }
}
