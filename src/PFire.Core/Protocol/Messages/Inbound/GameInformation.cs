namespace PFire.Core.Protocol.Messages.Inbound
{
    internal sealed class GameInformation : XFireMessage
    {
        public GameInformation() : base(XFireMessageType.GameInformation) {}

        [XMessageField("gameid")]
        public int GameId { get; private set; }

        [XMessageField("gip")]
        public int GameIP { get; private set; }

        [XMessageField("gport")]
        public int GamePort { get; private set; }
    }
}
