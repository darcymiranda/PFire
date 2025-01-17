using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PFire.Core.Protocol.Messages.Outbound;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Inbound
{
    internal sealed class FavoriteServerRemove : XFireMessage
    {
        public FavoriteServerRemove() : base(XFireMessageType.FavoriteServerRemove) { }

        [XMessageField("gameid")]
        public int GameId { get; set; }

        [XMessageField("gip")]
        public int GameIp { get; set; }

        [XMessageField("gport")]
        public int GamePort { get; set; }

        public async override Task Process(IXFireClient context)
        {
            await context.Server.Database.RemoveUserFavoriteServer(GameId, GameIp, GamePort, context.User.Id);
        }
    }
}
