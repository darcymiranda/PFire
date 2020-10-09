using PFire.Protocol.Messages.Outbound;
using PFire.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Protocol.Messages.Inbound
{
    public class GameInformation : IMessage
    {
        [XFireAttributeDef("gameid")]
        public int GameId { get; private set; }

        [XFireAttributeDef("gip")]
        public int GameIP { get; private set; }

        [XFireAttributeDef("gport")]
        public int GamePort { get; private set; }

        public short MessageTypeId => 4;

        public void Process(Context context)
        {
        }
    }
}
