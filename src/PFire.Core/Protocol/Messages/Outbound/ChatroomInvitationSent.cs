using System.Collections.Generic;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ChatroomInvitationSent : XFireMessage
    {
        public ChatroomInvitationSent(byte[] chatID, string chatTopic, int userId, string username, int unk47) : base(XFireMessageType.ChatroomInvitationSent)
        {
            //TODO: Figure out unknowns and once voice/broadcast functionality is added, factor that in.
            Cid = chatID;
            ChatRoomType = 1;
            UserId = userId;
            Username = username;
            Topic = chatTopic;
            Visibility = 1;
            VoiceEnabled = 0;
            Unk47 = unk47;
            Unk0D = "";
        }

        [XMessageField(0x04)]
        public byte[] Cid { get; set; }
        [XMessageField(0xaa)]
        public int ChatRoomType { get; set; }
        [XMessageField(0x01)]
        public int UserId { get; set; }
        [XMessageField(0x02)]
        public string Username { get; set; }
        [XMessageField(0x0d)]
        public string Unk0D { get; set; }
        [XMessageField(0x05)]
        public string Topic { get; set; }
        [XMessageField(0x47)]
        public int Unk47 { get; set; }
        [XMessageField(0x17)]
        public int Visibility { get; set; }
        [XMessageField(0x25)]
        public byte VoiceEnabled { get; set; }

        //This one was observed on an older version of xfire, but not 1.555, keeping it here for perservation purposes.
        //[XMessageField(0x46)]
        //public int Unk { get; set; }
    }
}
