using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFire.Core.Protocol.Messages.MessageEnums
{
    internal enum ChatMessageType : byte
    {
        Content = 0,
        Acknowledgement,
        ClientInformation,
        TypingNotification
    }
}
