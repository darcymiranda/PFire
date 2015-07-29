using PFire.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Protocol.Messages.Outbound
{
    public class ClientPreferences : IMessage
    {
        [XFireAttributeDef(0x4c)]
        public Dictionary<byte, string> preferences { get; private set; }


        public short MessageTypeId
        {
            get { return 141; }
        }

        public void Process(Context context)
        {
            preferences = new Dictionary<byte, string>();
            preferences.Add(1, "0");
            preferences.Add(4, "0");
            preferences.Add(5, "0");
            preferences.Add(6, "1");
            preferences.Add(7, "0");
            preferences.Add(8, "0");
            preferences.Add(11, "0");
            preferences.Add(17, "0");
            preferences.Add(18, "0");
            preferences.Add(19, "0");
            preferences.Add(20, "0");
            preferences.Add(21, "0");
        }
    }
}
