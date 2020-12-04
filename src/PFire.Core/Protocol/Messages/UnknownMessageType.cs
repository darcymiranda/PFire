using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Core.Protocol.Messages
{
    public class UnknownMessageType : XFireMessage
    {
        public UnknownMessageType() : base(XFireMessageType.Unknown)
        {
            AttributeValues = new Dictionary<string, dynamic>();
        }

        
        public int MessageCode { get; set; }


        public Dictionary<string, dynamic> AttributeValues;
    }
}
