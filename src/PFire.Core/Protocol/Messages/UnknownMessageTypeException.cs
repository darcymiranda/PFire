using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Protocol
{
    public class UnknownMessageTypeException : Exception
    {
        public UnknownMessageTypeException(short messageType) : base("Unknown message type " + messageType) { }
    }
}
