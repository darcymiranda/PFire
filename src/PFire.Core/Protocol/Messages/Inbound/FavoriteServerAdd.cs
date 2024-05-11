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
    internal sealed class FavoriteServerAdd : XFireMessage
    {
        public FavoriteServerAdd() : base(XFireMessageType.FavoriteServerAdd) { }

        [XMessageField("gameid")]
        public int GameId { get; set; }

        [XMessageField("gip")]
        public int GameIp { get; set; }

        [XMessageField("gport")]
        public int GamePort { get; set; }

        public async override Task Process(IXFireClient context)
        {
            var userFavoriteServers = await context.Server.Database.GetAllUserFavoriteServers(context.User);
            if (userFavoriteServers.Count < 30) // Check if the count is less than 30 (xfire limit pre-shutdown, this probably could be higher)
            {
                await context.Server.Database.AddUserFavoriteServer(GameId, GameIp, GamePort, context.User.Id);
            }
        }
    }
}
