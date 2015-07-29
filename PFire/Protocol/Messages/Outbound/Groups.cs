using PFire.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Protocol.Messages.Outbound
{
    public class Groups : IMessage
    {
        [XFireAttributeDef(0x19)]
        public List<int> GroupIds { get; private set; }

        [XFireAttributeDef(0x1a)]
        public List<string> GroupNames { get; private set; }

        public short MessageTypeId
        {
            get { return 151; }
        }

        public void Process(Context context)
        {
            GroupIds = new List<int>();
            GroupNames = new List<string>();
            //GroupIds.Add(1);
            //GroupNames.Add("Test");
        }
    }
}
