using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PFire.Core.Models;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class FriendsGamesInfo : XFireMessage
    {
        private readonly List<UserModel> _users;

        public FriendsGamesInfo(List<UserModel> users) : base(XFireMessageType.FriendsGameInfo)
        {
            _users = users;
            SessionIds = new List<Guid>();
            GameID = new List<int>();
            GameIP = new List<int>();
            GamePort = new List<int>();
        }

        [XMessageField("sid")]
        public List<Guid> SessionIds { get; set; }

        [XMessageField("gameid")]
        public List<int> GameID { get; set; }

        [XMessageField("gip")]
        public List<int> GameIP { get; set; }

        [XMessageField("gport")]
        public List<int> GamePort { get; set; }

        public override Task Process(IXFireClient client)
        {
            foreach (var user in _users)
            {
                if (user.ShowGameStatusToFriends == true)
                {
                    SessionIds.Add(client.Server.GetSession(user).SessionId);
                    GameID.Add(user.Game.Id);

                    if (user.ShowGameServerData == true)
                    {
                        GameIP.Add(user.Game.Ip);
                        GamePort.Add(user.Game.Port);
                    }
                }
            }

            return Task.CompletedTask;
        }

    }
}
