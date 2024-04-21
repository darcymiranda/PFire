using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PFire.Core.Models;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ChatroomInfoOnLogin : XFireMessage
    {
        public ChatroomInfoOnLogin() : base(XFireMessageType.ChatroomInfoOnLogin) {
            Cids = new List<byte[]>();
            Topics = new List<string>();
            ExpirationDate = new List<int>();
            MemberCount = new List<int>();
            Accessiblity = new List<int>();
            DateCreatedMaybe = new List<int>();
            ChatroomType = new List<int>();
        }

        [XMessageField(0x04)]
        public List<byte[]> Cids { get; set; }
        [XMessageField(0xaa)]
        public List<int> ChatroomType { get; set; }
        [XMessageField(0x05)]
        public List<string> Topics { get; set; }
        [XMessageField(0x14)]
        public List<int> ExpirationDate { get; set; }
        [XMessageField(0x15)]
        public List<int> MemberCount { get; set; }
        [XMessageField(0x17)]
        public List<int> Accessiblity { get; set; }
        [XMessageField(0x5D)]
        public List<int> DateCreatedMaybe { get; set; }

        public override async Task Process(IXFireClient context)
        {
            List<ChatroomModel> chatrooms = await context.Server.Database.GetAllChatrooms();

            foreach (ChatroomModel chatroom in chatrooms)
            {
                if (chatroom == null)
                {
                    return;
                }

                Cids.Add(chatroom.Id);
                Topics.Add(chatroom.Name);
                MemberCount.Add(chatroom.Users.Count);
                Accessiblity.Add(chatroom.Visibility);

                ExpirationDate.Add(0);
                DateCreatedMaybe.Add(1);
                ChatroomType.Add(1); //TODO: Function once broadcast packets are done.
            }
        }
    }
}
