using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PFire.Protocol.Messages.Outbound;
using PFire.Session;

namespace PFire.Protocol.Messages.Inbound
{
    public class ClientConfiguration : IMessage
    {
        [XFireAttributeDef("lang")]
        public string Language { get; set; }

        [XFireAttributeDef("skin")]
        public string Skin { get; set; }

        [XFireAttributeDef("theme")]
        public string Theme { get; set; }

        [XFireAttributeDef("partner")]
        public string Partner { get; set; }

        public short MessageTypeId => 16;

        public void Process(Context context)
        {
            context.SendAndProcessMessage(new Did());
        }
    }
}
