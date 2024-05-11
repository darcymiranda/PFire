using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Storage.Json;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ChatroomVoiceStatusChanged : XFireMessage
    {
        public ChatroomVoiceStatusChanged(byte[] chatID, byte active, int unk2f, int unk70, int unk71) : base(XFireMessageType.ChatroomVoiceStatusChange)
        {
            ChatId = chatID;
            //TODO: ALL OF THIS
            VoiceServerType = 1;
            IsVoiceServerActive = active;
            VoiceHost = unk2f;
            MaxVoiceUsers = 3;
            VoiceBandwidth = 3;
            Unk5F = chatID;
            Unk70 = unk70;
            Unk71 = unk71;
            Users.Add(1);
        }

        [XMessageField(0x04)]       
        public byte[] ChatId { get; set; }
        [XMessageField(0x34)]
        public int VoiceServerType { get; set; }
        [XMessageField(0x25)]
        public byte IsVoiceServerActive { get; set; }
        [XMessageField(0x2f)]
        public int VoiceHost { get; set; }
        [XMessageField(0x41)]
        public int MaxVoiceUsers { get; set; }
        [XMessageField(0x37)]
        public int VoiceBandwidth { get; set; }
        [XMessageField(0x5f)]
        public byte[] Unk5F { get; set; }
        [XMessageField(0x70)]
        public int Unk70 { get; set; }
        [XMessageField(0x71)]
        public int Unk71 { get; set; }
        [XMessageField(0x33)]
        public List<int> Users { get; set; } = new List<int>();
    }
}
