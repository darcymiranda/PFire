using PFire.Protocol.Messages;
using System;

namespace PFire.Client
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public IMessage MessageReceived { get; set; }
    }
}
