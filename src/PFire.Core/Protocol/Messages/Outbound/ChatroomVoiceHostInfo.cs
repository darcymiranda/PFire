namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class ChatroomVoiceHostInfo : XFireMessage
    {
        public ChatroomVoiceHostInfo(byte[] chatID, int userid, int voiceip, int voiceport, int unk3a, int unk3b, int unk3c, string unk3e) : base(XFireMessageType.ChatroomVoiceHostInfo)
        {
            ChatId = chatID;
            UserId = userid;
            VoiceIp = voiceip;
            VoicePort = voiceport;
            Unk3a = unk3a;
            Unk3b = unk3b;
            Unk3c = unk3c;
            Unk3e = unk3e;
        }
        [XMessageField(0x01)]
        public int UserId { get; set; }
        [XMessageField(0x04)]
        public byte[] ChatId { get; set; }
        [XMessageField(0x38)]
        public int VoiceIp { get; set; }
        [XMessageField(0x39)]
        public int VoicePort { get; set; }
        [XMessageField(0x3a)]
        public int Unk3a { get; set; }
        [XMessageField(0x3b)]
        public int Unk3b { get; set; }
        [XMessageField(0x3c)]
        public int Unk3c { get; set; }
        [XMessageField(0x3e)]
        public string Unk3e { get; set; }
    }
}
