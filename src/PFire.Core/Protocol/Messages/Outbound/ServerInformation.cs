using PFire.Core.Protocol.Messages;
using PFire.Database;
using PFire.Protocol;
using System.Collections.Generic;


namespace PFire.Core.Protocol.Messages.Outbound
{
    public class ServerInformation : XFireMessage
    {
        public ServerInformation() : base(XFireMessageType.ServerInfo)
        {

        }

        /// <summary>
        /// The PFireServer instance.
        /// </summary>
        [XMessageField]
        public PFireServer FireServer { get; set; }

        /// <summary>
        /// The PFire Database
        /// </summary>
        [XMessageField]
        public PFireDatabase DB { get { return FireServer.Database; } }
    }
}
