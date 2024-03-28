using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using PFire.Core.Session;

namespace PFire.Core.Protocol.Messages.Outbound
{
    internal sealed class GameServerSendFriendsFavorites : XFireMessage
    {
        public GameServerSendFriendsFavorites(int gameId) : base(XFireMessageType.GameServerSendFriendsFavorites)
        {
            GameId = gameId;
            GameIps = new List<int>();
            GamePorts = new List<int>();
            FriendIds = new List<List<int>>();
        }

        [XMessageField("gameid")]
        public int GameId { get; set; }

        [XMessageField("gip")]
        public List<int> GameIps { get; set; }

        [XMessageField("gport")]
        public List<int> GamePorts { get; set; }

        [XMessageField("friends")]
        public List<List<int>> FriendIds { get; set; }

        public override Task Process(IXFireClient context)
        {
            //TODO: Have a Database of IPs and Ports, with a user id who favorited it that is fetched by gameid
            //      Probably reads from the favorites database based on userid and gameid.
            //      
            //      You will need to start off processing with making a new temporary List<int> to put inside of FriendIds to satisfy the nested list need, this will hold the userid who favorited the server.
            //      Start off with sending back the GameId sent
            //      Iterate that list of favorites into Ips and Ports (unsigned ints on both) and your new userid List<List>
            //      If no hits, send GameIps and GamePorts with empty List<int>s, for FriendIds send back an empty List<List<int>> regardless, because the client expects a response.

            return Task.CompletedTask;
        }
    }
}
