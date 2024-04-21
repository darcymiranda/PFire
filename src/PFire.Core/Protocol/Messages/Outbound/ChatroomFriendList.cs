using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PFire.Core.Models;
using PFire.Core.Protocol.Messages.Inbound;
using PFire.Core.Session;
using PFire.Infrastructure.Entities;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ChatRoomFriendList : XFireMessage
    {
        private ChatroomModel _room;
        public ChatRoomFriendList(ChatroomModel room = null) : base(XFireMessageType.ChatroomFriendList)
        {
            if (room != null)
            {
                _room = room;
            }
        }

        [XMessageField(0x04)]
        public byte[] ChatId { get; set; }
        [XMessageField(0x05)]
        public string Topic { get; set; }
        [XMessageField(0x4D)]
        public string Motd { get; set; }
        [XMessageField(0x4E)]
        public int AllowVoiceChat { get; set; }
        [XMessageField(0x49)]
        public int DefaultPerm { get; set; }
        [XMessageField(0x14)]
        public int TimeStamp { get; set; }
        [XMessageField(0x17)]
        public int RoomType { get; set; }
        [XMessageField(0x33)]
        public List<int> UserIds { get; set; } = new List<int>();
        [XMessageField(0x44)]
        public List<int> Perms { get; set; } = new List<int>();
        [XMessageField(0x42)]
        public List<string> Names { get; set; } = new List<string>();
        [XMessageField(0x43)]
        public List<string> Nicks { get; set; } = new List<string>();
        [XMessageField(0x32)]
        public List<int> UsersInVoice { get; set; } = new List<int>();
        [XMessageField(0x2f)]
        public int Unk2F { get; set; }
        [XMessageField(0x37)]
        public int VoiceBandwith { get; set; }
        [XMessageField(0x5d)]
        public byte Unk5D { get; set; }
        public override async Task Process(IXFireClient context)
        {
            ChatId = _room.Id;
            Topic = _room.Name;
            Motd = _room.MOTD;
            AllowVoiceChat = 1;
            DefaultPerm = _room.DefaultPerms;
            TimeStamp = 1;
            RoomType = 1; //TODO: Once broadcasting packets are done, finish this.
            VoiceBandwith = 3;

            var userids = await context.Server.Database.QueryCurrentChatroomUsers(_room.Id);
            var users = await context.Server.Database.QueryUsers(userids);
            var highestLevels = await context.Server.Database.GetUsersHighestChatPowerLevels(ChatId, userids);
            foreach (var user in users)
            {
                UserIds.Add(user.Id);
                var userHighestLevel = highestLevels.FirstOrDefault(x => x.UserId == user.Id);
                Perms.Add(userHighestLevel != default ? userHighestLevel.HighestLevel : 2);
                Names.Add(user.Username);
                Nicks.Add(user.Nickname);
            }

            //TODO: Figure these unknowns out
            Unk2F = 0;
            Unk5D = 0;
        }
    }
}
